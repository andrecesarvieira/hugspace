namespace SynQcore.Domain.Entities.Communication;

/// <summary>
/// Representa um endorsement corporativo com categorias específicas para ambiente empresarial
/// </summary>
public class Endorsement : BaseEntity
{
    // Relacionamentos - Conteúdo endossado
    public Guid? PostId { get; set; }
    public Post? Post { get; set; }
    public Guid? CommentId { get; set; }
    public Comment? Comment { get; set; }
    
    // Relacionamentos - Quem endossa
    public Guid EndorserId { get; set; }
    public Employee Endorser { get; set; } = null!;
    
    // Tipo de endorsement corporativo
    public EndorsementType Type { get; set; } = EndorsementType.Helpful;
    
    // Metadata corporativa
    public string? Note { get; set; } // Nota opcional do endorser
    public bool IsPublic { get; set; } = true; // Visível para outros funcionários
    public DateTime EndorsedAt { get; set; } = DateTime.UtcNow;
    
    // Para analytics corporativas
    public string? Context { get; set; } // Contexto: "knowledge_sharing", "problem_solving", etc.
}

/// <summary>
/// Tipos de endorsement corporativo focados em ambiente empresarial
/// </summary>
public enum EndorsementType
{
    Helpful = 0,        // 🔥 Útil - conteúdo resolve problema/dúvida
    Insightful = 1,     // 💡 Perspicaz - traz nova perspectiva valiosa  
    Accurate = 2,       // ✅ Preciso - informação correta e confiável
    Innovative = 3,     // 🚀 Inovador - ideia criativa/solução nova
    Comprehensive = 4,  // 📚 Abrangente - cobre o tópico completamente
    WellResearched = 5, // 🔍 Bem Pesquisado - fontes sólidas e dados
    Actionable = 6,     // ⚡ Aplicável - pode ser implementado facilmente
    Strategic = 7       // 🎯 Estratégico - alinhado com objetivos corporativos
}