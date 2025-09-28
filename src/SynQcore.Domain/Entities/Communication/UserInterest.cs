namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa os interesses de um usuário para personalização do feed
/// Usado pelo algoritmo de recomendação de conteúdo
/// </summary>
public class UserInterest : BaseEntity
{
    /// <summary>
    /// ID do funcionário proprietário deste interesse.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Tipo de interesse being trackado.
    /// </summary>
    public InterestType Type { get; set; }

    /// <summary>
    /// Valor específico do interesse (nome da tag, categoria, departamento, etc.).
    /// </summary>
    public string InterestValue { get; set; } = string.Empty;

    /// <summary>
    /// Score de intensidade do interesse (0.0 a 10.0).
    /// </summary>
    public double Score { get; set; } = 1.0;

    /// <summary>
    /// Número de interações que geraram este interesse.
    /// </summary>
    public int InteractionCount { get; set; }

    /// <summary>
    /// Data e hora da última interação relacionada a este interesse.
    /// </summary>
    public DateTime LastInteractionAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fonte que originou este interesse.
    /// </summary>
    public InterestSource Source { get; set; }

    /// <summary>
    /// Indica se o interesse foi definido explicitamente pelo usuário.
    /// </summary>
    public bool IsExplicit { get; set; }

    /// <summary>
    /// Funcionário proprietário deste interesse.
    /// </summary>
    public Employee User { get; set; } = null!;
}

/// <summary>
/// Tipos de interesse que podem ser trackados
/// </summary>
public enum InterestType
{
    /// <summary>
    /// Interesse em tags específicas.
    /// </summary>
    Tag = 0,

    /// <summary>
    /// Interesse em categorias de conhecimento.
    /// </summary>
    Category = 1,

    /// <summary>
    /// Interesse em conteúdo de departamentos específicos.
    /// </summary>
    Department = 2,

    /// <summary>
    /// Interesse em conteúdo de autores específicos.
    /// </summary>
    Author = 3,

    /// <summary>
    /// Interesse em tipos específicos de posts.
    /// </summary>
    PostType = 4,

    /// <summary>
    /// Interesse em habilidades ou competências técnicas.
    /// </summary>
    Skill = 5
}

/// <summary>
/// Como o interesse foi identificado
/// </summary>
public enum InterestSource
{
    /// <summary>
    /// Interesse definido manualmente pelo usuário.
    /// </summary>
    UserDefined = 0,

    /// <summary>
    /// Interesse inferido baseado no histórico de visualizações.
    /// </summary>
    ViewHistory = 1,

    /// <summary>
    /// Interesse inferido baseado no histórico de curtidas.
    /// </summary>
    LikeHistory = 2,

    /// <summary>
    /// Interesse inferido baseado no histórico de comentários.
    /// </summary>
    CommentHistory = 3,

    /// <summary>
    /// Interesse inferido baseado no histórico de buscas.
    /// </summary>
    SearchHistory = 4,

    /// <summary>
    /// Interesse baseado nas habilidades do perfil profissional.
    /// </summary>
    ProfileSkills = 5
}
