namespace SynQcore.Domain.Entities.Communication;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TagType Type { get; set; } = TagType.General;

    public string Color { get; set; } = "#6B7280";

    public int UsageCount { get; set; }

    public ICollection<PostTag> PostTags { get; set; } = [];
}

public enum TagType
{
    General = 0,

    Skill = 1,

    Technology = 2,

    Department = 3,

    Project = 4,

    Policy = 5
}
