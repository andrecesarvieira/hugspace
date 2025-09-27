/*
 * SynQcore - Corporate Social Network API
 *
 * Copyright (c) 2025 André César Vieira
 *
 * Licensed under the MIT License
 * Author: André César Vieira <andrecesarvieira@hotmail.com>
 * GitHub: https://github.com/andrecesarvieira/synqcore
 *
 * This file is part of SynQcore, an open-source corporate social network API
 * built with Clean Architecture, .NET 9, and PostgreSQL.
 */

using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using SynQcore.Infrastructure.Data;
using SynQcore.Api.Middleware;
using AspNetCoreRateLimit;
using SynQcore.Infrastructure.Services.Auth;
using SynQcore.Common;
using SynQcore.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using SynQcore.Application.Commands.Auth;
using MediatR;
using FluentValidation;
using SynQcore.Application.Behaviors;


// Configure Serilog for corporate logging with audit trails
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)

    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        formatProvider: System.Globalization.CultureInfo.InvariantCulture,
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj} " +
        "| CorrelationId: {CorrelationId} | UserId: {UserId} | IP: {ClientIP}{NewLine}{Exception}")
    .WriteTo.File("logs/synqcore-corporate-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        formatProvider: System.Globalization.CultureInfo.InvariantCulture,
        outputTemplate:
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj} " +
            "| CorrelationId: {CorrelationId} | UserId: {UserId} | IP: {ClientIP} " +
            "| {SourceContext}{NewLine}{Exception}")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Current User Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SynQcore.Application.Common.Interfaces.ICurrentUserService, SynQcore.Infrastructure.Services.CurrentUserService>();

// Discussion Thread Helper
builder.Services.AddScoped<SynQcore.Application.Common.Helpers.DiscussionThreadHelper>();

// Role Initialization Service
builder.Services.AddScoped<RoleInitializationService>();

// Admin Bootstrap Service
builder.Services.AddScoped<AdminBootstrapService>();

// Use Serilog as the logging provider
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI Corporate
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SynQcore Corporate API",
        Description = "API de rede social corporativa com .NET 9 e Clean Architecture. Endpoints requerem autenticação JWT exceto /health.",
        Contact = new OpenApiContact
        {
            Name = "André César Vieira",
            Email = "andrecesarvieira@hotmail.com",
            Url = new Uri("https://github.com/andrecesarvieira/synqcore")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });


    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Resolver conflitos de schema IDs usando nomes completos
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));

    // Incluir comentários XML na documentação Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add DbContext (Unified with Identity)
builder.Services.AddDbContext<SynQcoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register DbContext interface
builder.Services.AddScoped<SynQcore.Application.Common.Interfaces.ISynQcoreDbContext>(provider =>
    provider.GetRequiredService<SynQcoreDbContext>());

// Identity Configuration
builder.Services.AddIdentity<ApplicationUserEntity, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<SynQcoreDbContext>()
.AddDefaultTokenProviders();


// JWT Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JWT");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };

    // Enable JWT authentication for SignalR hubs
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            // If the request is for SignalR hub, get the token from query string
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Configure CORS for Corporate environment
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorporatePolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Production CORS - restrict to corporate domains
            policy.WithOrigins("https://synqcore.company.com", "https://app.synqcore.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Add API versioning (Asp.Versioning 8.x)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.QueryStringApiVersionReader("version"),
        new Asp.Versioning.HeaderApiVersionReader("X-Version")
    );
}).AddMvc();

// Add API Explorer for Swagger (Asp.Versioning 8.x)
builder.Services.AddApiVersioning().AddMvc().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgresql")
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "redis");

// Register corporate middleware services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add memory cache (required for rate limiting)
builder.Services.AddMemoryCache();

// Add corporate rate limiting
builder.Services.AddCorporateRateLimit(builder.Configuration);

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

// Add SignalR for corporate real-time communication
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB limit for corporate messages
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(10);
    options.KeepAliveInterval = TimeSpan.FromMinutes(5);
});

// Mapeamento manual implementado via extensions - sem AutoMapper

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SynQcore Corporate API v1");
        options.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(-1); // Hide schemas by default
    });
}

// HTTPS redirection apenas em produção para evitar warnings em desenvolvimento
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Corporate middleware pipeline
app.UseExceptionHandler(); // Global exception handler

// Rate limiting middleware (com bypass inteligente)
app.UseCorporateRateLimit(); // Corporate rate limiting com bypass

app.UseMiddleware<AuditLoggingMiddleware>(); // Corporate audit logging

app.UseCors("CorporatePolicy");

// Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hubs
app.MapHub<SynQcore.Api.Hubs.CorporateCollaborationHub>("/hubs/corporate-collaboration");
app.MapHub<SynQcore.Api.Hubs.ExecutiveCommunicationHub>("/hubs/executive-communication");

app.MapControllers();

// Health checks endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            version = SynQcoreInfo.Version,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description
            }),
            timestamp = DateTime.UtcNow
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

try
{
    Log.Information("Starting SynQcore Corporate API");

    // Inicializar roles corporativas
    using (var scope = app.Services.CreateScope())
    {
        await RoleInitializationService.InitializeAsync(scope.ServiceProvider);
        // Criar administrador padrão se não existir nenhum
        await AdminBootstrapService.InitializeAsync(scope.ServiceProvider);
    }

    // Abrir Swagger automaticamente no navegador padrão em desenvolvimento
    if (app.Environment.IsDevelopment())
    {
        var serverUrls = app.Urls;
        var baseUrl = serverUrls.FirstOrDefault() ?? "http://localhost:5000";
        var swaggerUrl = $"{baseUrl}/swagger";

        // Executar após um pequeno delay para garantir que o servidor esteja pronto
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000); // Aguarda 2 segundos
            try
            {
                // Detectar sistema operacional e abrir o navegador apropriado
                if (OperatingSystem.IsWindows())
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = swaggerUrl,
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsLinux())
                {
                    System.Diagnostics.Process.Start("xdg-open", swaggerUrl);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    System.Diagnostics.Process.Start("open", swaggerUrl);
                }
                Log.Information("Swagger UI aberto automaticamente: {SwaggerUrl}", swaggerUrl);
            }
            catch (Exception ex)
            {
                Log.Warning("Não foi possível abrir o Swagger automaticamente: {Error}", ex.Message);
            }
        });
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SynQcore Corporate API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
