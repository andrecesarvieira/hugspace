using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

/// <summary>
/// Categoria de dados pessoais para classificação LGPD/GDPR
/// Define tipos de dados coletados e suas finalidades
/// </summary>
public class PersonalDataCategory : BaseEntity
{
    /// <summary>
    /// Nome da categoria de dados
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da categoria
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Nível de sensibilidade dos dados
    /// </summary>
    public SensitivityLevel SensitivityLevel { get; set; } = SensitivityLevel.Normal;

    /// <summary>
    /// Finalidade do tratamento destes dados
    /// </summary>
    public string ProcessingPurpose { get; set; } = string.Empty;

    /// <summary>
    /// Base legal para o tratamento (LGPD Art. 7º)
    /// </summary>
    public LegalBasisForProcessing LegalBasis { get; set; } = LegalBasisForProcessing.Consent;

    /// <summary>
    /// Tempo de retenção dos dados (em meses)
    /// </summary>
    public int RetentionPeriodMonths { get; set; }

    /// <summary>
    /// Se requer consentimento explícito
    /// </summary>
    public bool RequiresConsent { get; set; } = true;

    /// <summary>
    /// Se é obrigatório para o funcionamento do sistema
    /// </summary>
    public bool IsMandatoryData { get; set; }

    /// <summary>
    /// Se pode ser compartilhado com terceiros
    /// </summary>
    public bool AllowsSharing { get; set; }

    /// <summary>
    /// Se está sujeito a transferência internacional
    /// </summary>
    public bool InternationalTransfer { get; set; }

    /// <summary>
    /// Países autorizados para transferência
    /// </summary>
    public string? AuthorizedCountries { get; set; }

    /// <summary>
    /// Medidas de segurança específicas
    /// </summary>
    public string? SecurityMeasures { get; set; }

    /// <summary>
    /// Se a categoria está ativa
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Campos de dados incluídos nesta categoria
    /// </summary>
    public string IncludedFields { get; set; } = string.Empty;

    /// <summary>
    /// Versão da categoria (para controle de mudanças)
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Data de entrada em vigor
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Observações adicionais
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Níveis de sensibilidade de dados pessoais
/// </summary>
public enum SensitivityLevel
{
    Normal = 0,
    Sensitive = 1,
    HighlySensitive = 2,
    Restricted = 3
}

/// <summary>
/// Bases legais para tratamento de dados pessoais (LGPD Art. 7º)
/// </summary>
public enum LegalBasisForProcessing
{
    Consent = 0,
    LegalObligation = 1,
    ContractExecution = 2,
    LegitimateRights = 3,
    VitalInterests = 4,
    HealthProtection = 5,
    LegitimateInterest = 6,
    CreditProtection = 7,
    PublicPolicies = 8
}
