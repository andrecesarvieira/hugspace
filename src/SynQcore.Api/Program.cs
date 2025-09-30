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
using SynQcore.Application.Services;
using SynQcore.Infrastructure.Services;
using SynQcore.Common;
using SynQcore.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using SynQcore.Application.Commands.Auth;
using MediatR;
using SynQcore.Application.Features.Privacy.DTOs;
using SynQcore.Application.Features.Privacy.Queries;
using SynQcore.Application.Features.Privacy.Commands;
using SynQcore.Application.Features.Privacy.Handlers;
using FluentValidation;
using SynQcore.Application.Behaviors;
using SynQcore.Application.Common.DTOs;
using SynQcore.Application.Features.MediaAssets.Queries;
using SynQcore.Application.Features.MediaAssets.Commands;
using SynQcore.Application.Features.MediaAssets.DTOs;
using SynQcore.Application.Features.MediaAssets.Handlers;
using SynQcore.Application.Features.DocumentTemplates.Queries;
using SynQcore.Application.Features.DocumentTemplates.Commands;
using SynQcore.Application.Features.DocumentTemplates.DTOs;
using SynQcore.Application.Features.DocumentTemplates.Handlers;
using SynQcore.Application.Features.CorporateSearch.DTOs;
using SynQcore.Application.Features.CorporateSearch.Queries;
using SynQcore.Application.Features.CorporateSearch.Handlers;
using SynQcore.Application.Features.Moderation.DTOs;
using SynQcore.Application.Features.Moderation.Queries;
using SynQcore.Application.Features.Moderation.Handlers;

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

// Audit Service (Fase 6 - Security & Moderation)
builder.Services.AddScoped<SynQcore.Application.Services.IAuditService, SynQcore.Infrastructure.Services.AuditService>();

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

// Add Health Checks - Skip external dependencies in Testing environment
if (!builder.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgresql")
        .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "redis");
}
else
{
    // Simple health checks for testing
    builder.Services.AddHealthChecks();
}

// Register corporate middleware services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add memory cache (required for rate limiting)
builder.Services.AddMemoryCache();

// Add corporate rate limiting - Skip in Testing environment
if (!builder.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddCorporateRateLimit(builder.Configuration);
}

// Add security headers configuration
builder.Services.AddSecurityHeaders(builder.Configuration);

// Security Services (comentado temporariamente até resolver conflitos)
// builder.Services.AddScoped<IInputSanitizationService, InputSanitizationService>();
// builder.Services.AddScoped<ISecurityMonitoringService, SecurityMonitoringService>();
// builder.Services.AddScoped<IAdvancedRateLimitingService, AdvancedRateLimitingService>();

// Add MediatR - Registrar handlers da Application layer
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});

// Registrar handlers manualmente para garantir que sejam encontrados

// === MEDIA ASSETS HANDLERS ===
builder.Services.AddScoped<IRequestHandler<GetMediaAssetsQuery, PagedResult<MediaAssetDto>>, GetMediaAssetsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMediaAssetByIdQuery, MediaAssetDetailDto?>, GetMediaAssetByIdQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMediaAssetsByTypeQuery, PagedResult<MediaAssetDto>>, GetMediaAssetsByTypeQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetDepartmentGalleryQuery, PagedResult<MediaAssetDto>>, GetDepartmentGalleryQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetPopularAssetsQuery, List<MediaAssetDto>>, GetPopularAssetsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetRecentAssetsQuery, List<MediaAssetDto>>, GetRecentAssetsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMyAssetsQuery, PagedResult<MediaAssetDto>>, GetMyAssetsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMediaAssetFileQuery, MediaAssetFileDto?>, GetMediaAssetFileQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMediaAssetThumbnailQuery, MediaAssetThumbnailDto?>, GetMediaAssetThumbnailQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMediaAssetStatsQuery, MediaAssetStatsDto?>, GetMediaAssetStatsQueryHandler>();

// MediaAssets Command Handlers
builder.Services.AddScoped<IRequestHandler<UploadMediaAssetCommand, MediaAssetDto>, UploadMediaAssetCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateMediaAssetCommand, MediaAssetDto?>, UpdateMediaAssetCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DeleteMediaAssetCommand, bool>, DeleteMediaAssetCommandHandler>();
builder.Services.AddScoped<IRequestHandler<BulkUploadMediaAssetsCommand, List<MediaAssetDto>>, BulkUploadMediaAssetsCommandHandler>();
builder.Services.AddScoped<IRequestHandler<RegisterMediaAssetAccessCommand, bool>, RegisterMediaAssetAccessCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GenerateThumbnailCommand, bool>, GenerateThumbnailCommandHandler>();

// === DOCUMENT TEMPLATES HANDLERS ===
builder.Services.AddScoped<IRequestHandler<GetTemplatesQuery, PagedResult<DocumentTemplateDto>>, GetTemplatesQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetTemplateByIdQuery, DocumentTemplateDetailDto?>, GetTemplateByIdQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetTemplateUsageStatsQuery, TemplateUsageStatsDto?>, GetTemplateUsageStatsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetTemplatesByCategoryQuery, PagedResult<DocumentTemplateDto>>, GetTemplatesByCategoryQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetActiveTemplatesQuery, List<DocumentTemplateDto>>, GetActiveTemplatesQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetPopularTemplatesQuery, List<DocumentTemplateDto>>, GetPopularTemplatesQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetMyTemplatesQuery, PagedResult<DocumentTemplateDto>>, GetMyTemplatesQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetDepartmentTemplatesQuery, List<DocumentTemplateDto>>, GetDepartmentTemplatesQueryHandler>();

// DocumentTemplates Command Handlers
builder.Services.AddScoped<IRequestHandler<CreateTemplateCommand, DocumentTemplateDto>, CreateTemplateCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateTemplateCommand, DocumentTemplateDto?>, UpdateTemplateCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DeleteTemplateCommand, bool>, DeleteTemplateCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CreateDocumentFromTemplateCommand, CreateDocumentFromTemplateDto?>, CreateDocumentFromTemplateCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DuplicateTemplateCommand, DocumentTemplateDto?>, DuplicateTemplateCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ToggleTemplateStatusCommand, DocumentTemplateDto?>, ToggleTemplateStatusCommandHandler>();

// === CORPORATE SEARCH HANDLERS ===
builder.Services.AddScoped<IRequestHandler<CorporateSearchQuery, PagedResult<SearchResultDto>>, CorporateSearchQueryHandler>();
builder.Services.AddScoped<IRequestHandler<AdvancedSearchQuery, PagedResult<SearchResultDto>>, AdvancedSearchQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetSearchSuggestionsQuery, List<SearchSuggestionDto>>, GetSearchSuggestionsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetSearchAnalyticsQuery, SearchAnalyticsDto>, GetSearchAnalyticsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetTrendingTopicsQuery, List<TrendingTopicDto>>, GetTrendingTopicsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetContentStatsQuery, ContentStatsDto>, GetContentStatsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetSearchConfigQuery, SearchConfigDto>, GetSearchConfigQueryHandler>();

// === MODERATION HANDLERS ===
builder.Services.AddScoped<IRequestHandler<GetModerationAuditLogsQuery, PagedResult<ModerationAuditLogDto>>, ModerationQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetModerationDashboardStatsQuery, ModerationDashboardStatsDto>, GetModerationDashboardStatsQueryHandler>();

// === PRIVACY & LGPD HANDLERS ===
// Privacy Query Handlers
builder.Services.AddScoped<IRequestHandler<GetPersonalDataCategoriesQuery, PagedResult<PersonalDataCategoryDto>>, PrivacyQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetPersonalDataCategoryByIdQuery, PersonalDataCategoryDto?>, PrivacyQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetConsentRecordsQuery, PagedResult<ConsentRecordDto>>, PrivacyQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetConsentRecordByIdQuery, ConsentRecordDto?>, PrivacyQueryHandler>();

// Privacy Create Command Handlers
builder.Services.AddScoped<IRequestHandler<CreatePersonalDataCategoryCommand, PersonalDataCategoryDto>, PrivacyCreateHandlers>();
builder.Services.AddScoped<IRequestHandler<CreateConsentRecordCommand, ConsentRecordDto>, PrivacyCreateHandlers>();
builder.Services.AddScoped<IRequestHandler<CreateDataExportRequestCommand, DataExportRequestDto>, PrivacyCreateHandlers>();
builder.Services.AddScoped<IRequestHandler<CreateDataDeletionRequestCommand, DataDeletionRequestDto>, PrivacyCreateHandlers>();

// Privacy Update Command Handlers
builder.Services.AddScoped<IRequestHandler<UpdatePersonalDataCategoryCommand, PersonalDataCategoryDto?>, PrivacyUpdateHandlers>();
builder.Services.AddScoped<IRequestHandler<UpdateConsentRecordCommand, ConsentRecordDto?>, PrivacyUpdateHandlers>();

// OBRIGATÓRIO: Registro manual dos handlers de notificação (Fase 5.0)
// Configure rate limiting with corporate thresholds

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
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker" || app.Environment.EnvironmentName == "Testing")
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

// Security headers middleware
app.UseSecurityHeaders(); // Corporate security headers

// Rate limiting middleware (com bypass inteligente) - Skip in Testing environment
if (!app.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    app.UseCorporateRateLimit(); // Corporate rate limiting com bypass
}

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

    // Inicializar roles corporativas - Skip em ambiente de Testing
    if (!app.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
    {
        using (var scope = app.Services.CreateScope())
        {
            await RoleInitializationService.InitializeAsync(scope.ServiceProvider);
            // Criar administrador padrão se não existir nenhum
            await AdminBootstrapService.InitializeAsync(scope.ServiceProvider);
        }
    }

    // REMOVIDO: Abertura automática do Swagger (agora controlada pelo script start-full.py)
    // Para abrir manualmente: http://localhost:5000/swagger
    /*
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
    */

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

/// <summary>
/// Classe Program explícita para permitir acesso em testes de integração
/// </summary>
public partial class Program { }
