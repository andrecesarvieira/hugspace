namespace SynQcore.Domain.Entities.Communication;

public class Endorsement : BaseEntity
{
    public Guid? PostId { get; set; }

    public Post? Post { get; set; }

    public Guid? CommentId { get; set; }

    public Comment? Comment { get; set; }

    public Guid EndorserId { get; set; }

    public Employee Endorser { get; set; } = null!;

    public EndorsementType Type { get; set; } = EndorsementType.Helpful;

    public string? Note { get; set; }

    public bool IsPublic { get; set; } = true;

    public DateTime EndorsedAt { get; set; } = DateTime.UtcNow;

    public string? Context { get; set; }
}

public enum EndorsementType
{
    Helpful = 0,

    Insightful = 1,

    Accurate = 2,

    Innovative = 3,

    Comprehensive = 4,

    WellResearched = 5,

    Actionable = 6,

    Strategic = 7
}
