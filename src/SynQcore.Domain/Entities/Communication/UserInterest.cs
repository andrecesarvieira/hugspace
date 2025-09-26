namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa os interesses de um usuário para personalização do feed
/// Usado pelo algoritmo de recomendação de conteúdo
/// </summary>
public class UserInterest : BaseEntity
{
    // Identificação
    public Guid UserId { get; set; }
    public InterestType Type { get; set; }
    public string InterestValue { get; set; } = string.Empty; // Tag, categoria, departamento, etc.
    
    // Métricas de interesse
    public double Score { get; set; } = 1.0; // Intensidade do interesse (0-10)
    public int InteractionCount { get; set; } // Quantas interações
    public DateTime LastInteractionAt { get; set; } = DateTime.UtcNow;
    
    // Origem do interesse
    public InterestSource Source { get; set; }
    public bool IsExplicit { get; set; } // Usuario definiu explicitamente
    
    // Relacionamentos
    public Employee User { get; set; } = null!;
}

/// <summary>
/// Tipos de interesse que podem ser trackados
/// </summary>
public enum InterestType
{
    Tag = 0,           // Interesse em tags específicas
    Category = 1,      // Interesse em categorias
    Department = 2,    // Interesse em departamentos
    Author = 3,        // Interesse em autores específicos
    PostType = 4,      // Interesse em tipos de post
    Skill = 5          // Interesse em skills técnicas
}

/// <summary>
/// Como o interesse foi identificado
/// </summary>
public enum InterestSource
{
    UserDefined = 0,   // Usuário definiu manualmente
    ViewHistory = 1,   // Baseado em visualizações
    LikeHistory = 2,   // Baseado em curtidas
    CommentHistory = 3, // Baseado em comentários
    SearchHistory = 4,  // Baseado em buscas
    ProfileSkills = 5   // Baseado no perfil profissional
}