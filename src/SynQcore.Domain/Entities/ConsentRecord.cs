using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Registro de consentimento do usuário para diferentes categorias de dados pessoais
/// Implementa compliance LGPD/GDPR para rastreabilidade de consentimentos
/// </summary>
public class ConsentRecord : BaseEntity
{
    /// <summary>
    /// ID do funcionário que deu o consentimento
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Funcionário associado ao consentimento
    /// </summary>
    public virtual Employee Employee { get; set; } = null!;

    /// <summary>
    /// Categoria de dados pessoais para a qual o consentimento foi dado
    /// </summary>
    public string ConsentCategory { get; set; } = string.Empty;

    /// <summary>
    /// Finalidade específica do tratamento dos dados
    /// </summary>
    public string ProcessingPurpose { get; set; } = string.Empty;

    /// <summary>
    /// Se o consentimento foi concedido ou negado
    /// </summary>
    public bool ConsentGranted { get; set; }

    /// <summary>
    /// Data e hora em que o consentimento foi registrado
    /// </summary>
    public DateTime ConsentDate { get; set; }

    /// <summary>
    /// Data de expiração do consentimento (se aplicável)
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Versão dos termos de consentimento
    /// </summary>
    public string TermsVersion { get; set; } = string.Empty;

    /// <summary>
    /// Evidência de como o consentimento foi coletado
    /// </summary>
    public string CollectionEvidence { get; set; } = string.Empty;

    /// <summary>
    /// Endereço IP de onde o consentimento foi dado
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User-Agent do navegador usado para dar o consentimento
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Se o registro está ativo (não foi revogado)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Data da última modificação do consentimento
    /// </summary>
    public DateTime? LastModificationDate { get; set; }

    /// <summary>
    /// Observações adicionais sobre o consentimento
    /// </summary>
    public string? Notes { get; set; }
}
