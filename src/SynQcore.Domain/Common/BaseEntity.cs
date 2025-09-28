namespace SynQcore.Domain.Common;

/// <summary>
/// Classe base abstrata para todas as entidades do domínio SynQcore.
/// Fornece propriedades comuns como identificação, timestamps e controle de exclusão lógica.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único da entidade.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Data e hora de criação da entidade em UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data e hora da última atualização da entidade em UTC.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Indica se a entidade foi excluída logicamente.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Data e hora da exclusão lógica da entidade em UTC, se aplicável.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Construtor protegido que inicializa os timestamps de criação.
    /// </summary>
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca a entidade como excluída logicamente, definindo IsDeleted como true
    /// e configurando DeletedAt com o timestamp atual.
    /// </summary>
    public void MarkAsDeleted()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdateTimestamp();

    }

    /// <summary>
    /// Restaura a entidade de uma exclusão lógica, definindo IsDeleted como false
    /// e removendo o timestamp de exclusão.
    /// </summary>
    public void RestoreFromDeletion()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAt = null;
        UpdateTimestamp();
    }

    /// <summary>
    /// Atualiza o timestamp de modificação para o momento atual em UTC.
    /// </summary>
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
