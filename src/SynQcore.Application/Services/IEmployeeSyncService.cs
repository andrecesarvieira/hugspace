using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Application.Services;

/// <summary>
/// Serviço para sincronização automática entre AspNetUsers e Employees
/// Garante integridade referencial e consistência de dados
/// </summary>
public interface IEmployeeSyncService
{
    /// <summary>
    /// Garante que existe um Employee correspondente ao usuário Identity.
    /// Se não existir, cria automaticamente baseado nos dados do Identity.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Employee existente ou recém-criado</returns>
    Task<Employee> EnsureEmployeeExistsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo Employee baseado nos dados do usuário Identity
    /// </summary>
    /// <param name="userId">ID do usuário Identity</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Employee criado</returns>
    Task<Employee> CreateEmployeeFromIdentityUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sincroniza todos os usuários do AspNetUsers com Employees
    /// Usado para correção em massa de dados históricos
    /// </summary>
    /// <returns>Número de registros sincronizados</returns>
    Task<int> SyncAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se o Employee está sincronizado com os dados do Identity
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se estiver sincronizado</returns>
    Task<bool> IsEmployeeInSyncAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza dados do Employee baseado nas informações mais recentes do Identity
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Employee atualizado</returns>
    Task<Employee> UpdateEmployeeFromIdentityAsync(Guid userId, CancellationToken cancellationToken = default);    /// <summary>
                                                                                                                   /// Verifica integridade completa da sincronização
                                                                                                                   /// Retorna estatísticas de sincronização
                                                                                                                   /// </summary>
                                                                                                                   /// <returns>Estatísticas de sincronização</returns>
    Task<EmployeeSyncStats> GetSyncStatsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Estatísticas de sincronização entre AspNetUsers e Employees
/// </summary>
public class EmployeeSyncStats
{
    public int TotalUsers { get; set; }
    public int TotalEmployees { get; set; }
    public int SyncedUsers { get; set; }
    public int MissingEmployees { get; set; }
    public int OrphanedEmployees { get; set; }
    public DateTime LastSyncCheck { get; set; }
    public List<Guid> UsersNeedingSync { get; set; } = new();
    public List<Guid> OrphanedEmployeeIds { get; set; } = new();
}
