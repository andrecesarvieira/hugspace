using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Solicitação de exportação de dados pessoais conforme LGPD/GDPR
/// Implementa direito de portabilidade de dados do titular
/// </summary>
public class DataExportRequest : BaseEntity
{
    /// <summary>
    /// ID do funcionário que solicitou a exportação
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário que solicitou a exportação
    /// </summary>
    public virtual Employee Employee { get; set; } = null!;

    /// <summary>
    /// Status da solicitação de exportação
    /// </summary>
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    /// <summary>
    /// Data da solicitação
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Data de processamento da solicitação
    /// </summary>
    public DateTime? ProcessingDate { get; set; }

    /// <summary>
    /// Data de conclusão da exportação
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Categorias de dados solicitadas para exportação
    /// </summary>
    public string DataCategories { get; set; } = string.Empty;

    /// <summary>
    /// Formato desejado para exportação (JSON, PDF, CSV)
    /// </summary>
    public ExportFormat Format { get; set; } = ExportFormat.JSON;

    /// <summary>
    /// Caminho do arquivo gerado (se aplicável)
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Hash do arquivo para verificação de integridade
    /// </summary>
    public string? FileHash { get; set; }

    /// <summary>
    /// Tamanho do arquivo em bytes
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// Data de expiração do link de download
    /// </summary>
    public DateTime? DownloadExpirationDate { get; set; }

    /// <summary>
    /// Motivo da solicitação (opcional)
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Observações do processamento
    /// </summary>
    public string? ProcessingNotes { get; set; }

    /// <summary>
    /// ID do usuário que processou a solicitação
    /// </summary>
    public Guid? ProcessedById { get; set; }

    /// <summary>
    /// Usuário que processou a solicitação
    /// </summary>
    public virtual Employee? ProcessedBy { get; set; }

    /// <summary>
    /// Endereço IP de origem da solicitação
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Número de tentativas de download
    /// </summary>
    public int DownloadAttempts { get; set; }
}

/// <summary>
/// Status possíveis para solicitações de dados
/// </summary>
public enum RequestStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Rejected = 3,
    Expired = 4,
    Cancelled = 5
}

/// <summary>
/// Formatos disponíveis para exportação de dados
/// </summary>
public enum ExportFormat
{
    JSON = 0,
    PDF = 1,
    CSV = 2,
    XML = 3
}
