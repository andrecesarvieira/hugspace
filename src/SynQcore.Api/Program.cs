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

using SynQcore.Infrastructure.Data;
using SynQcore.Api.Middleware;
using SynQcore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using SynQcore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        Description = @"
    **🚀 SynQcore - Corporate Social Network API**
    
    Created by **André César Vieira** - Enterprise Software Architect
    
    ### About This API
    Open-source corporate social network platform built with Clean Architecture,
    .NET 9, and PostgreSQL. Designed for enterprise-grade performance and scalability.
    
    ### Key Features
    - 🏢 Employee management and corporate directory
    - 💬 Corporate posts, discussions, and collaboration
    - 👥 Team management and reporting structures  
    - 🔔 Real-time notification system
    - 📊 Department and organizational analytics
    - 🔒 JWT authentication with role-based access
    - ⚡ Redis caching for optimal performance
    - 📝 Complete audit trails and logging
    - 🛡️ Corporate-grade rate limiting
    
    ### Technology Stack
    - **.NET 9** - Latest Microsoft framework
    - **PostgreSQL 16** - Enterprise database
    - **Redis 7** - High-performance caching
    - **Clean Architecture** - Maintainable and testable
    - **CQRS Pattern** - Scalable command/query separation
    - **Docker** - Containerized deployment
    
    ### Authentication & Security
    All endpoints require JWT Bearer token except health checks and documentation.
    Rate limiting varies by user role (Employee: 100/min, Manager: 300/min, HR: 500/min, Admin: 1000/min).
    
    ### Open Source
    This project is open source under MIT License. 
    ⭐ Star the repository: https://github.com/andrecesarvieira/synqcore
            ",
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
});

// Add DbContext (Unified with Identity)
builder.Services.AddDbContext<SynQcoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// JWT Configuration - ADICIONAR
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

// Add corporate rate limiting
builder.Services.AddCorporateRateLimit(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SynQcore Corporate API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(-1); // Hide schemas by default
    });
}

app.UseHttpsRedirection();

// Corporate middleware pipeline
app.UseExceptionHandler(); // Global exception handler
app.UseMiddleware<AuditLoggingMiddleware>(); // Corporate audit logging
app.UseCorporateRateLimit(); // Corporate rate limiting

app.UseCors("CorporatePolicy");

// Add authentication & authorization middleware (quando implementado)
// app.UseAuthentication();
// app.UseAuthorization();

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
