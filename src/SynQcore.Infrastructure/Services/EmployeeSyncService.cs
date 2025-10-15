using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SynQcore.Application.Common.Interfaces;
using SynQcore.Application.Services;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de sincronização entre AspNetUsers e Employees
/// </summary>
public partial class EmployeeSyncService : IEmployeeSyncService
{
    private readonly ISynQcoreDbContext _context;
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly ILogger<EmployeeSyncService> _logger;

    public EmployeeSyncService(
        ISynQcoreDbContext context,
        UserManager<ApplicationUserEntity> userManager,
        ILogger<EmployeeSyncService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Employee> EnsureEmployeeExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        LogEnsureEmployeeStarted(_logger, userId);

        // Verificar se Employee já existe
        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        if (existingEmployee != null)
        {
            LogEmployeeAlreadyExists(_logger, userId);
            return existingEmployee;
        }

        // Criar Employee a partir do AspNetUsers
        var newEmployee = await CreateEmployeeFromIdentityUserAsync(userId, cancellationToken);

        LogEmployeeEnsured(_logger, userId, newEmployee.Email);
        return newEmployee;
    }

    public async Task<Employee> CreateEmployeeFromIdentityUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        LogCreatingEmployeeFromIdentity(_logger, userId);

        // Buscar dados do usuário no Identity
        var identityUser = await _userManager.FindByIdAsync(userId.ToString());
        if (identityUser == null)
        {
            LogIdentityUserNotFound(_logger, userId);
            throw new InvalidOperationException($"Usuário do Identity não encontrado: {userId}");
        }

        // Extrair nome do email se não houver FirstName/LastName
        var email = identityUser.Email ?? string.Empty;
        var emailParts = email.Split('@');
        var namePart = emailParts.Length > 0 ? emailParts[0] : "Usuario";

        // Tentar extrair nome e sobrenome do email ou usar dados do Identity
        string firstName, lastName;

        if (!string.IsNullOrEmpty(identityUser.UserName) && identityUser.UserName.Contains(' '))
        {
            var nameParts = identityUser.UserName.Split(' ', 2);
            firstName = nameParts[0];
            lastName = nameParts.Length > 1 ? nameParts[1] : "";
        }
        else if (namePart.Contains('.'))
        {
            var namePartsFromEmail = namePart.Split('.');
            firstName = CapitalizeFirst(namePartsFromEmail[0]);
            lastName = namePartsFromEmail.Length > 1 ? CapitalizeFirst(namePartsFromEmail[1]) : "";
        }
        else
        {
            firstName = CapitalizeFirst(namePart);
            lastName = "Sistema";
        }

        // Gerar EmployeeId único
        var employeeId = await GenerateUniqueEmployeeIdAsync(firstName, lastName, cancellationToken);

        // Criar novo Employee
        var employee = new Employee
        {
            Id = userId,
            EmployeeId = employeeId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            JobTitle = DetermineJobTitleFromEmail(email),
            Position = "Colaborador",
            HireDate = DateTime.UtcNow, // Data de criação como hire date
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // Salvar no banco
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        LogEmployeeCreatedFromIdentity(_logger, userId, employee.Email, employee.EmployeeId);
        return employee;
    }

    public async Task<int> SyncAllUsersAsync(CancellationToken cancellationToken = default)
    {
        LogSyncAllUsersStarted(_logger);

        var stats = await GetSyncStatsAsync(cancellationToken);
        var syncCount = 0;

        if (stats.UsersNeedingSync.Count == 0)
        {
            LogNoUsersNeedSync(_logger);
            return 0;
        }

        LogFoundUsersNeedingSync(_logger, stats.UsersNeedingSync.Count);

        // Processar em lotes para evitar problemas de memória
        const int batchSize = 50;
        var batches = stats.UsersNeedingSync.Chunk(batchSize);

        foreach (var batch in batches)
        {
            foreach (var userId in batch)
            {
                try
                {
                    await CreateEmployeeFromIdentityUserAsync(userId, cancellationToken);
                    syncCount++;
                }
                catch (Exception ex)
                {
                    LogErrorSyncingUser(_logger, userId, ex);
                    // Continuar com os outros usuários
                }
            }

            // Pequena pausa entre lotes
            await Task.Delay(100, cancellationToken);
        }

        LogSyncAllUsersCompleted(_logger, syncCount);
        return syncCount;
    }

    public async Task<bool> IsEmployeeInSyncAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Verificar se usuário existe no Identity
        var identityUser = await _userManager.FindByIdAsync(userId.ToString());
        if (identityUser == null)
            return false;

        // Verificar se Employee correspondente existe
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        return employee != null;
    }

    public async Task<Employee> UpdateEmployeeFromIdentityAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        LogUpdatingEmployeeFromIdentity(_logger, userId);

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        if (employee == null)
        {
            LogEmployeeNotFoundForUpdate(_logger, userId);
            throw new InvalidOperationException($"Employee com ID {userId} não encontrado para atualização.");
        }

        var identityUser = await _userManager.FindByIdAsync(userId.ToString());
        if (identityUser == null)
        {
            LogIdentityUserNotFoundForUpdate(_logger, userId);
            throw new InvalidOperationException($"Usuário Identity com ID {userId} não encontrado para atualização.");
        }

        // Atualizar campos que podem ter mudado
        var hasChanges = false;

        if (employee.Email != identityUser.Email)
        {
            employee.Email = identityUser.Email ?? employee.Email;
            hasChanges = true;
        }

        if (hasChanges)
        {
            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            LogEmployeeUpdatedFromIdentity(_logger, userId);
        }

        return employee;
    }

    public async Task<EmployeeSyncStats> GetSyncStatsAsync(CancellationToken cancellationToken = default)
    {
        LogGatheringSyncStats(_logger);

        // Buscar todos os usuários do Identity
        var allUsers = await _userManager.Users
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        var totalUsers = allUsers.Count;

        // Buscar todos os Employees
        var allEmployees = await _context.Employees
            .Where(e => !e.IsDeleted)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var totalEmployees = allEmployees.Count;

        // Identificar usuários que precisam de sincronização
        var usersNeedingSync = allUsers
            .Where(userId => !allEmployees.Contains(userId))
            .ToList();

        // Identificar Employees órfãos (sem usuário correspondente no Identity)
        var orphanedEmployees = allEmployees
            .Where(employeeId => !allUsers.Contains(employeeId))
            .ToList();

        var stats = new EmployeeSyncStats
        {
            TotalUsers = totalUsers,
            TotalEmployees = totalEmployees,
            SyncedUsers = totalUsers - usersNeedingSync.Count,
            MissingEmployees = usersNeedingSync.Count,
            OrphanedEmployees = orphanedEmployees.Count,
            LastSyncCheck = DateTime.UtcNow,
            UsersNeedingSync = usersNeedingSync,
            OrphanedEmployeeIds = orphanedEmployees
        };

        LogSyncStatsGathered(_logger, stats.TotalUsers, stats.TotalEmployees, stats.MissingEmployees, stats.OrphanedEmployees);
        return stats;
    }

    // === MÉTODOS AUXILIARES ===

    private async Task<string> GenerateUniqueEmployeeIdAsync(string firstName, string lastName, CancellationToken cancellationToken)
    {
        var baseId = GenerateEmployeeIdFromName(firstName, lastName);
        var employeeId = baseId;
        var counter = 1;

        // Verificar se já existe e gerar variação se necessário
        while (await _context.Employees.AnyAsync(e => e.EmployeeId == employeeId, cancellationToken))
        {
            employeeId = $"{baseId}{counter:D3}";
            counter++;

            // Evitar loop infinito
            if (counter > 999)
            {
                employeeId = $"{baseId}{Guid.NewGuid().ToString()[..8].ToUpper(CultureInfo.InvariantCulture)}";
                break;
            }
        }

        return employeeId;
    }

    private static string GenerateEmployeeIdFromName(string firstName, string lastName)
    {
        // Gerar ID no formato: Primeiras 3 letras do nome + Primeiras 3 do sobrenome + número
        var firstPart = firstName.Length >= 3 ? firstName[..3].ToUpper(CultureInfo.InvariantCulture) : firstName.PadRight(3, 'X').ToUpper(CultureInfo.InvariantCulture);
        var lastPart = string.IsNullOrEmpty(lastName) ? "SYS" :
                      lastName.Length >= 3 ? lastName[..3].ToUpper(CultureInfo.InvariantCulture) : lastName.PadRight(3, 'X').ToUpper(CultureInfo.InvariantCulture);

        return $"{firstPart}{lastPart}001";
    }

    private static string CapitalizeFirst(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Length == 1 ?
            input.ToUpper(CultureInfo.InvariantCulture) :
            char.ToUpper(input[0], CultureInfo.InvariantCulture) + input[1..].ToLower(CultureInfo.InvariantCulture);
    }

    private static string DetermineJobTitleFromEmail(string email)
    {
        if (email.Contains("admin"))
            return "Administrador do Sistema";
        if (email.Contains("manager") || email.Contains("gestor"))
            return "Gerente";
        if (email.Contains("dev") || email.Contains("developer"))
            return "Desenvolvedor";
        if (email.Contains("support") || email.Contains("suporte"))
            return "Suporte";
        if (email.Contains("rh") || email.Contains("hr"))
            return "Recursos Humanos";

        return "Colaborador";
    }

    // === LoggerMessage delegates para performance ===

    [LoggerMessage(EventId = 4001, Level = LogLevel.Information,
        Message = "Iniciando verificação de Employee para usuário {UserId}")]
    private static partial void LogEnsureEmployeeStarted(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Information,
        Message = "Employee já existe para usuário {UserId}")]
    private static partial void LogEmployeeAlreadyExists(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4003, Level = LogLevel.Information,
        Message = "Employee garantido para usuário {UserId}: {Email}")]
    private static partial void LogEmployeeEnsured(ILogger logger, Guid userId, string email);

    [LoggerMessage(EventId = 4004, Level = LogLevel.Information,
        Message = "Criando Employee a partir do Identity para usuário {UserId}")]
    private static partial void LogCreatingEmployeeFromIdentity(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4005, Level = LogLevel.Warning,
        Message = "Usuário do Identity não encontrado: {UserId}")]
    private static partial void LogIdentityUserNotFound(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4006, Level = LogLevel.Information,
        Message = "Employee criado a partir do Identity - UserId: {UserId}, Email: {Email}, EmployeeId: {EmployeeId}")]
    private static partial void LogEmployeeCreatedFromIdentity(ILogger logger, Guid userId, string email, string employeeId);

    [LoggerMessage(EventId = 4007, Level = LogLevel.Information,
        Message = "Iniciando sincronização de todos os usuários")]
    private static partial void LogSyncAllUsersStarted(ILogger logger);

    [LoggerMessage(EventId = 4008, Level = LogLevel.Information,
        Message = "Nenhum usuário precisa de sincronização")]
    private static partial void LogNoUsersNeedSync(ILogger logger);

    [LoggerMessage(EventId = 4009, Level = LogLevel.Information,
        Message = "Encontrados {Count} usuários precisando de sincronização")]
    private static partial void LogFoundUsersNeedingSync(ILogger logger, int count);

    [LoggerMessage(EventId = 4010, Level = LogLevel.Error,
        Message = "Erro ao sincronizar usuário {UserId}")]
    private static partial void LogErrorSyncingUser(ILogger logger, Guid userId, Exception ex);

    [LoggerMessage(EventId = 4011, Level = LogLevel.Information,
        Message = "Sincronização completa - {SyncCount} usuários sincronizados")]
    private static partial void LogSyncAllUsersCompleted(ILogger logger, int syncCount);

    [LoggerMessage(EventId = 4012, Level = LogLevel.Information,
        Message = "Atualizando Employee a partir do Identity para usuário {UserId}")]
    private static partial void LogUpdatingEmployeeFromIdentity(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4013, Level = LogLevel.Warning,
        Message = "Employee não encontrado para atualização: {UserId}")]
    private static partial void LogEmployeeNotFoundForUpdate(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4014, Level = LogLevel.Warning,
        Message = "Usuário do Identity não encontrado para atualização: {UserId}")]
    private static partial void LogIdentityUserNotFoundForUpdate(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4015, Level = LogLevel.Information,
        Message = "Employee atualizado a partir do Identity: {UserId}")]
    private static partial void LogEmployeeUpdatedFromIdentity(ILogger logger, Guid userId);

    [LoggerMessage(EventId = 4016, Level = LogLevel.Information,
        Message = "Coletando estatísticas de sincronização")]
    private static partial void LogGatheringSyncStats(ILogger logger);

    [LoggerMessage(EventId = 4017, Level = LogLevel.Information,
        Message = "Estatísticas coletadas - Users: {TotalUsers}, Employees: {TotalEmployees}, Missing: {MissingEmployees}, Orphaned: {OrphanedEmployees}")]
    private static partial void LogSyncStatsGathered(ILogger logger, int totalUsers, int totalEmployees, int missingEmployees, int orphanedEmployees);
}
