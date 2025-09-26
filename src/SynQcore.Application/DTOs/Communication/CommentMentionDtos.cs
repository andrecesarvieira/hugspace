namespace SynQcore.Application.DTOs.Communication;

/// <summary>
/// DTO para visualização de menção em comentário
/// </summary>
public class CommentMentionDto
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public Guid MentionedEmployeeId { get; set; }
    public string MentionedEmployeeName { get; set; } = string.Empty;
    public string MentionText { get; set; } = string.Empty;
    public int StartPosition { get; set; }
    public int Length { get; set; }
    public bool HasBeenNotified { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string Context { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para criação de menção em comentário
/// </summary>
public class CreateCommentMentionDto
{
    public Guid MentionedEmployeeId { get; set; }
    public string MentionText { get; set; } = string.Empty;
    public int StartPosition { get; set; }
    public int Length { get; set; }
    public string Context { get; set; } = "General";
    public string Urgency { get; set; } = "Normal";
}

/// <summary>
/// DTO para notificação de menção
/// </summary>
public class MentionNotificationDto
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public string CommentContent { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public string PostTitle { get; set; } = string.Empty;
    public string MentionedByName { get; set; } = string.Empty;
    public string MentionText { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}