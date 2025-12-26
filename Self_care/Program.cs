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
var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

// Логируем все переменные окружения, связанные с ConnectionStrings
var envVar = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var configVar = builder.Configuration.GetConnectionString("DefaultConnection");

logger.LogInformation("=== ConnectionString Debug ===");
logger.LogInformation("Environment variable 'ConnectionStrings__DefaultConnection': {EnvVar}", 
    string.IsNullOrEmpty(envVar) ? "NULL or EMPTY" : $"FOUND (length: {envVar.Length})");
logger.LogInformation("Configuration 'ConnectionStrings:DefaultConnection': {Config}", 
    string.IsNullOrEmpty(configVar) ? "NULL or EMPTY" : $"FOUND (length: {configVar.Length})");

var connectionString = envVar ?? configVar;

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogError("Connection string 'DefaultConnection' not found!");
    logger.LogError("Available environment variables with 'Connection': {EnvVars}", 
        string.Join(", ", Environment.GetEnvironmentVariables().Keys.Cast<string>().Where(k => k.Contains("Connection", StringComparison.OrdinalIgnoreCase))));
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

// Преобразование ConnectionString из URL формата в key-value формат для Npgsql
if (connectionString.StartsWith("postgresql://") || connectionString.StartsWith("postgres://"))
{
    try
    {
        logger.LogInformation("Converting PostgreSQL URL format to connection string format");
        
        // Убираем префикс
        var url = connectionString.Replace("postgresql://", "").Replace("postgres://", "");
        
        // Парсим URL
        var parts = url.Split('@');
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid PostgreSQL URL format");
        }
        
        var userPass = parts[0].Split(':');
        var username = userPass[0];
        var password = userPass.Length > 1 ? userPass[1] : "";
        
        var hostDb = parts[1].Split('?')[0]; // Убираем query параметры
        var hostPortDb = hostDb.Split('/');
        var hostPort = hostPortDb[0].Split(':');
        var host = hostPort[0];
        var port = hostPort.Length > 1 ? hostPort[1] : "5432";
        var database = hostPortDb.Length > 1 ? hostPortDb[1] : "";
        
        // Извлекаем параметры из query string
        var sslMode = "Require";
        if (parts[1].Contains("?"))
        {
            var queryString = parts[1].Split('?')[1];
            var queryParams = queryString.Split('&');
            foreach (var param in queryParams)
            {
                if (param.StartsWith("sslmode=", StringComparison.OrdinalIgnoreCase))
                {
                    var sslValue = param.Substring("sslmode=".Length).Trim();
                    if (!string.IsNullOrEmpty(sslValue))
                    {
                        // Преобразуем в правильный формат
                        sslMode = sslValue.Equals("require", StringComparison.OrdinalIgnoreCase) ? "Require" :
                                  sslValue.Equals("prefer", StringComparison.OrdinalIgnoreCase) ? "Prefer" :
                                  sslValue.Equals("disable", StringComparison.OrdinalIgnoreCase) ? "Disable" :
                                  "Require";
                    }
                }
            }
        }
        
        // Создаем connection string в формате key-value
        connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode}";
        
        logger.LogInformation("Successfully converted URL to connection string format");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error converting PostgreSQL URL to connection string format. Using original string.");
        // Если не удалось преобразовать, используем исходную строку
    }
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