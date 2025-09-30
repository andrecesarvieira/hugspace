using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Solicitação de exclusão de dados pessoais conforme LGPD/GDPR
/// Implementa direito ao esquecimento do titular dos dados
/// </summary>
public class DataDeletionRequest : BaseEntity
{
    /// <summary>
    /// ID do funcionário que solicitou a exclusão
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário que solicitou a exclusão
    /// </summary>
    public virtual Employee Employee { get; set; } = null!;

    /// <summary>
    /// Status da solicitação de exclusão
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
    /// Data de conclusão da exclusão
    /// </summary>
    public DateTime? CompletionDate { get; set; }

    /// <summary>
    /// Tipo de exclusão solicitada
    /// </summary>
    public DeletionType DeletionType { get; set; } = DeletionType.PersonalData;

    /// <summary>
    /// Categorias específicas de dados para exclusão
    /// </summary>
    public string DataCategories { get; set; } = string.Empty;

    /// <summary>
    /// Motivo da solicitação de exclusão
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Justificativa legal para a exclusão
    /// </summary>
    public string? LegalJustification { get; set; }

    /// <summary>
    /// Se a exclusão deve incluir backups
    /// </summary>
    public bool IncludeBackups { get; set; } = true;

    /// <summary>
    /// Se a exclusão deve ser completa ou anonimização
    /// </summary>
    public bool CompleteDeletion { get; set; } = true;

    /// <summary>
    /// Período de carência antes da exclusão (em dias)
    /// </summary>
    public int GracePeriodDays { get; set; } = 30;

    /// <summary>
    /// Data efetiva da exclusão (após carência)
    /// </summary>
    public DateTime? EffectiveDeletionDate { get; set; }

    /// <summary>
    /// Relatório de itens excluídos
    /// </summary>
    public string? DeletionReport { get; set; }

    /// <summary>
    /// Hash de verificação da exclusão
    /// </summary>
    public string? VerificationHash { get; set; }

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
    /// Se foi enviada notificação de confirmação
    /// </summary>
    public bool NotificationSent { get; set; }

    /// <summary>
    /// Data de envio da notificação
    /// </summary>
    public DateTime? NotificationDate { get; set; }
}

/// <summary>
/// Tipos de exclusão disponíveis
/// </summary>
public enum DeletionType
{
    PersonalData = 0,
    CompleteAccount = 1,
    SpecificData = 2,
    Anonymization = 3
}
