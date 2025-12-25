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


// Получаем ConnectionString - сначала из переменной окружения, потом из конфигурации
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogError("Connection string 'DefaultConnection' not found!");
    logger.LogError("Environment variable ConnectionStrings__DefaultConnection: {EnvVar}", 
        Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? "null");
    logger.LogError("Configuration ConnectionStrings:DefaultConnection: {Config}", 
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "null");
    logger.LogError("Available configuration keys: {Keys}", 
        string.Join(", ", builder.Configuration.AsEnumerable().Select(k => k.Key)));
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Логируем ConnectionString (без пароля для безопасности)
var safeConnectionString = connectionString.Contains("Password=") 
    ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***"
    : (connectionString.Contains("@") 
        ? connectionString.Substring(0, connectionString.IndexOf("@") + 1) + "***"
        : connectionString);
logger.LogInformation("Using ConnectionString: {ConnectionString}", safeConnectionString);
logger.LogInformation("ConnectionString source: {Source}", 
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") != null ? "Environment Variable" : "Configuration");

// Если ConnectionString в формате URL без порта, добавляем порт
if (connectionString.StartsWith("postgresql://") && !connectionString.Contains(":5432") && !connectionString.Contains("?"))
{
    var atIndex = connectionString.IndexOf("@");
    var slashIndex = connectionString.IndexOf("/", atIndex);
    if (atIndex > 0 && slashIndex > atIndex)
    {
        connectionString = connectionString.Insert(slashIndex, ":5432");
        logger.LogInformation("Added port 5432 to ConnectionString");
    }
}

// Если ConnectionString в формате URL без sslmode, добавляем его
if (connectionString.StartsWith("postgresql://") && !connectionString.Contains("sslmode="))
{
    connectionString += (connectionString.Contains("?") ? "&" : "?") + "sslmode=require";
    logger.LogInformation("Added sslmode=require to ConnectionString");
}

builder.Services.AddDbContext<SelfCareDB>(options =>
{
    options.UseNpgsql(connectionString);
    logger.LogInformation("DbContext configured with ConnectionString");
});

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
    try
    {
        var dbContext = services.GetRequiredService<SelfCareDB>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var loggeres = services.GetRequiredService<ILogger<Program>>();
        loggeres.LogError(ex, "An error occurred while migrating the database.");
        // Не останавливаем приложение, если миграции не применились
        // Это позволит приложению запуститься даже если БД недоступна
    }
}

// CORS configuration - ДОЛЖНО БЫТЬ ПЕРЕД UseAuthentication и UseAuthorization
var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? "http://localhost:5173";
var allowedOrigins = frontendUrl.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
if (allowedOrigins.Length == 0)
{
    allowedOrigins = new[] { frontendUrl };
}

// Логирование для отладки CORS
var loggerу = app.Services.GetRequiredService<ILogger<Program>>();
loggerу.LogInformation("CORS: Allowed origins: {Origins}", string.Join(", ", allowedOrigins));

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