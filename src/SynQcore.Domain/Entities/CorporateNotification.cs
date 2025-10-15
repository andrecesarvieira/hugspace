using SynQcore.Domain.Common;

namespace SynQcore.Domain.Entities;

public class CorporateNotification : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public NotificationPriority Priority { get; set; }

    public NotificationStatus Status { get; set; }

    public Guid CreatedByEmployeeId { get; set; }

    public Employee CreatedByEmployee { get; set; } = null!;

    public Guid? TargetDepartmentId { get; set; }

    public Department? TargetDepartment { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset? ScheduledFor { get; set; }

    public bool RequiresApproval { get; set; }

    public Guid? ApprovedByEmployeeId { get; set; }

    public Employee? ApprovedByEmployee { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public bool RequiresAcknowledgment { get; set; }

    public NotificationChannels EnabledChannels { get; set; }

    public string? Metadata { get; set; }

    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
}

public class NotificationDelivery : BaseEntity
{
    public Guid NotificationId { get; set; }

    public CorporateNotification Notification { get; set; } = null!;

    public Guid EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public DeliveryStatus Status { get; set; }

    public NotificationChannel Channel { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }

    public DateTimeOffset? ReadAt { get; set; }

    public DateTimeOffset? AcknowledgedAt { get; set; }

    public int DeliveryAttempts { get; set; }

    public DateTimeOffset? NextAttemptAt { get; set; }

    public string? ErrorDetails { get; set; }

    public string? ChannelData { get; set; }
}

public class NotificationTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string TitleTemplate { get; set; } = string.Empty;

    public string ContentTemplate { get; set; } = string.Empty;

    public string? EmailTemplate { get; set; }

    public NotificationType DefaultType { get; set; }

    public NotificationPriority DefaultPriority { get; set; }

    public NotificationChannels DefaultChannels { get; set; }

    public bool DefaultRequiresApproval { get; set; }

    public bool DefaultRequiresAcknowledgment { get; set; }

    public bool IsActive { get; set; } = true;

    public string? AvailablePlaceholders { get; set; }
}
