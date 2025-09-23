namespace SynQcore.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

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

    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}