using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Self_care.Data;
using Self_care.Services;
using Self_care.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT configuration section is missing.");


// Получаем ConnectionString с логированием для отладки
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogError("Connection string 'DefaultConnection' not found!");
    logger.LogError("Available configuration keys: {Keys}", 
        string.Join(", ", builder.Configuration.AsEnumerable().Select(k => k.Key)));
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Логируем ConnectionString (без пароля для безопасности)
var safeConnectionString = connectionString.Contains("Password=") 
    ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***"
    : connectionString;
logger.LogInformation("Using ConnectionString: {ConnectionString}", safeConnectionString);

builder.Services.AddDbContext<SelfCareDB>(options =>
    options.UseNpgsql(connectionString));

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SelfCare API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase;
});


var app = builder.Build();

// Автоматическое применение миграций при старте (для Render без Shell)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // try
    // {
        var dbContext = services.GetRequiredService<SelfCareDB>();
        dbContext.Database.Migrate();
    // }
    // catch (Exception ex)
    // {
    //     var logger = services.GetRequiredService<ILogger<Program>>();
    //     logger.LogError(ex, "An error occurred while migrating the database.");
    //     // Не останавливаем приложение, если миграции не применились
    //     // Это позволит приложению запуститься даже если БД недоступна
    // }
}

// CORS configuration - ДОЛЖНО БЫТЬ ПЕРЕД UseAuthentication и UseAuthorization
var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? "http://localhost:5173";
var allowedOrigins = frontendUrl.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
if (allowedOrigins.Length == 0)
{
    allowedOrigins = new[] { frontendUrl };
}

// Логирование для отладки CORS
var loggere = app.Services.GetRequiredService<ILogger<Program>>();
loggere.LogInformation("CORS: Allowed origins: {Origins}", string.Join(", ", allowedOrigins));

app.UseCors(policy => policy
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
