namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa um endorsement corporativo com categorias específicas para ambiente empresarial
/// </summary>
public class Endorsement : BaseEntity
{
    /// <summary>
    /// ID do post endossado (se aplicável).
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// Post endossado (se aplicável).
    /// </summary>
    public Post? Post { get; set; }

    /// <summary>
    /// ID do comentário endossado (se aplicável).
    /// </summary>
    public Guid? CommentId { get; set; }

    /// <summary>
    /// Comentário endossado (se aplicável).
    /// </summary>
    public Comment? Comment { get; set; }

    /// <summary>
    /// ID do funcionário que fez o endorsement.
    /// </summary>
    public Guid EndorserId { get; set; }

    /// <summary>
    /// Funcionário que fez o endorsement.
    /// </summary>
    public Employee Endorser { get; set; } = null!;

    /// <summary>
    /// Tipo de endorsement corporativo aplicado.
    /// </summary>
    public EndorsementType Type { get; set; } = EndorsementType.Helpful;

    /// <summary>
    /// Nota opcional explicando o endorsement.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Indica se o endorsement é visível para outros funcionários.
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Data e hora quando o endorsement foi feito.
    /// </summary>
    public DateTime EndorsedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Contexto corporativo do endorsement para analytics.
    /// </summary>
    public string? Context { get; set; }
}

/// <summary>
/// Tipos de endorsement corporativo focados em ambiente empresarial
/// </summary>
public enum EndorsementType
{
    /// <summary>
    /// Conteúdo útil que resolve problemas ou dúvidas (🔥).
    /// </summary>
    Helpful = 0,

    /// <summary>
    /// Conteúdo perspicaz que traz nova perspectiva valiosa (💡).
    /// </summary>
    Insightful = 1,

    /// <summary>
    /// Informação precisa, correta e confiável (✅).
    /// </summary>
    Accurate = 2,

    /// <summary>
    /// Ideia inovadora ou solução criativa (🚀).
    /// </summary>
    Innovative = 3,

    /// <summary>
    /// Conteúdo abrangente que cobre o tópico completamente (📚).
    /// </summary>
    Comprehensive = 4,

    /// <summary>
    /// Conteúdo bem pesquisado com fontes sólidas (🔍).
    /// </summary>
    WellResearched = 5,

    /// <summary>
    /// Solução aplicável que pode ser implementada facilmente (⚡).
    /// </summary>
    Actionable = 6,

    /// <summary>
    /// Conteúdo estratégico alinhado com objetivos corporativos (🎯).
    /// </summary>
    Strategic = 7
}
