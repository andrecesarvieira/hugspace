using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.KnowledgeManagement.DTOs;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public TagType Type { get; set; }
    public string Color { get; set; } = null!;
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public TagType Type { get; set; } = TagType.General;
    public string Color { get; set; } = "#6B7280";
}

public class UpdateTagDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TagType? Type { get; set; }
    public string? Color { get; set; }
}