using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Controle de acesso granular para documentos corporativos
/// Define quem pode acessar cada documento específico
/// </summary>
public class DocumentAccess : BaseEntity
{
    /// <summary>
    /// ID do documento ao qual o acesso se refere
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// ID do funcionário que tem acesso (null = acesso por departamento/role)
    /// </summary>
    public Guid? EmployeeId { get; set; }

    /// <summary>
    /// ID do departamento que tem acesso (null = acesso individual)
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Role que tem acesso (null = acesso específico)
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Tipo de acesso concedido
    /// </summary>
    public AccessType AccessType { get; set; }

    /// <summary>
    /// Data de expiração do acesso (null = sem expiração)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// ID do funcionário que concedeu o acesso
    /// </summary>
    public Guid GrantedByEmployeeId { get; set; }

    /// <summary>
    /// Data em que o acesso foi concedido
    /// </summary>
    public DateTimeOffset GrantedAt { get; set; }

    /// <summary>
    /// Motivo da concessão de acesso
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Indica se o acesso está ativo
    /// </summary>
    public bool IsActive { get; set; }

    // Relacionamentos
    /// <summary>
    /// Documento ao qual o acesso se refere
    /// </summary>
    public CorporateDocument Document { get; set; } = null!;

    /// <summary>
    /// Funcionário que tem acesso
    /// </summary>
    public Employee? Employee { get; set; }

    /// <summary>
    /// Departamento que tem acesso
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// Funcionário que concedeu o acesso
    /// </summary>
    public Employee GrantedByEmployee { get; set; } = null!;
}

/// <summary>
/// Log de acessos a documentos para auditoria corporativa
/// </summary>
public class DocumentAccessLog : BaseEntity
{
    /// <summary>
    /// ID do documento acessado
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// ID do funcionário que acessou
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Tipo de ação realizada no documento
    /// </summary>
    public DocumentAction Action { get; set; }

    /// <summary>
    /// Data e hora do acesso
    /// </summary>
    public DateTimeOffset AccessedAt { get; set; }

    /// <summary>
    /// Endereço IP de origem do acesso
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent do navegador/aplicação
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Informações adicionais sobre o acesso
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Duração da sessão de visualização (em segundos)
    /// </summary>
    public int? SessionDurationSeconds { get; set; }

    // Relacionamentos
    /// <summary>
    /// Documento que foi acessado
    /// </summary>
    public CorporateDocument Document { get; set; } = null!;

    /// <summary>
    /// Funcionário que realizou o acesso
    /// </summary>
    public Employee Employee { get; set; } = null!;
}