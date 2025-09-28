namespace SynQcore.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdateTimestamp();

    }

    public void RestoreFromDeletion()
    {
        if (!IsDeleted)
            return;

        IsDeleted = false;
        DeletedAt = null;
        UpdateTimestamp();
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
