using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Infrastructure.Data;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.IntegrationTests.Infrastructure;

/// <summary>
/// Factory customizada para testes de integração
/// Configura ambiente de teste com banco em memória
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly object SeedLock = new object();
    private static bool _isSeeded;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove TODOS os serviços relacionados ao DbContext e Entity Framework
            var descriptorsToRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<SynQcoreDbContext>) ||
                d.ServiceType == typeof(SynQcoreDbContext) ||
                d.ServiceType.Name.Contains("DbContext") ||
                d.ServiceType.Name.Contains("EntityFramework") ||
                d.ServiceType.Name.Contains("Npgsql") ||
                d.ImplementationType?.Name.Contains("DbContext") == true ||
                d.ImplementationType?.Name.Contains("EntityFramework") == true ||
                d.ImplementationType?.Name.Contains("Npgsql") == true
            ).ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Adiciona banco de dados em memória para testes (substituindo completamente)
            services.AddDbContext<SynQcoreDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase_Shared");
                // Habilita logging sensível para testes
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Registra a interface ISynQcoreDbContext para resolução de dependência
            services.AddScoped<SynQcore.Application.Common.Interfaces.ISynQcoreDbContext>(provider =>
                provider.GetRequiredService<SynQcoreDbContext>());

            // Remove logs desnecessários durante testes
            services.RemoveAll(typeof(ILogger<>));
            services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Error));
        });

        builder.UseEnvironment("Testing");
        builder.UseContentRoot(Directory.GetCurrentDirectory());
    }

    /// <summary>
    /// Obtém um cliente HTTP configurado com dados de teste inicializados
    /// </summary>
    public new HttpClient CreateClient()
    {
        var client = base.CreateClient();
        EnsureDatabase();
        return client;
    }

    /// <summary>
    /// Garante que o banco de dados esteja criado e populado
    /// </summary>
    private void EnsureDatabase()
    {
        lock (SeedLock)
        {
            if (_isSeeded)
                return;

            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SynQcoreDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUserEntity>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Garante que o banco foi criado
            context.Database.EnsureCreated();

            // Popula dados de teste
            SeedTestDataAsync(context, userManager, roleManager).GetAwaiter().GetResult();

            _isSeeded = true;
        }
    }

    /// <summary>
    /// Popula banco de teste com dados básicos
    /// </summary>
    private static async Task SeedTestDataAsync(SynQcoreDbContext context, UserManager<ApplicationUserEntity> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        // Cria roles
        var roles = new[] { "Admin", "Employee", "Manager", "HR" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName) { Id = Guid.NewGuid() });
            }
        }

        // Cria usuário admin
        var adminUserId = Guid.NewGuid();
        var adminUser = new ApplicationUserEntity
        {
            Id = adminUserId,
            UserName = "admin",
            Email = "admin@synqcore.com",
            EmailConfirmed = true,
            EmployeeId = adminUserId
        };

        var result = await userManager.CreateAsync(adminUser, "SynQcore@Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Cria funcionário correspondente
        var adminEmployee = new Employee
        {
            Id = adminUserId, // Mesmo ID do usuário
            EmployeeId = "admin",
            Email = "admin@synqcore.com",
            FirstName = "Administrador",
            LastName = "Sistema",
            JobTitle = "Administrador do Sistema",
            Position = "Administrador",
            IsActive = true,
            HireDate = DateTime.UtcNow.AddYears(-1)
        };

        context.Employees.Add(adminEmployee);
        await context.SaveChangesAsync();
    }
}
