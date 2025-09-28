namespace SynQcore.Domain.Entities;

public enum NotificationType
{
    CompanyAnnouncement = 1,

    PolicyUpdate = 2,

    Emergency = 3,

    SystemNotification = 4,

    HumanResources = 5,

    DepartmentUpdate = 6,

    ProjectUpdate = 7,

    Security = 8,

    ExecutiveCommunication = 9,

    Training = 10
}

public enum NotificationPriority
{
    Low = 1,

    Normal = 2,

    High = 3,

    Critical = 4,

    Emergency = 5
}

public enum NotificationStatus
{
    Draft = 1,

    Scheduled = 2,

    PendingApproval = 3,

    Approved = 4,

    Sending = 5,

    Sent = 6,

    Rejected = 7,

    Cancelled = 8,

    Expired = 9,

    Failed = 10
}

[Flags]
public enum NotificationChannels
{
    None = 0,

    InApp = 1,

    Email = 2,

    Push = 4,

    SMS = 8,

    Webhook = 16,

    Teams = 32,

    Slack = 64,

    All = InApp | Email | Push | SMS | Webhook | Teams | Slack
}

public enum NotificationChannel
{
    InApp = 1,

    Email = 2,

    MobilePush = 3,

    BrowserPush = 4,

    SMS = 5,

    Webhook = 6,

    Teams = 7,

    Slack = 8
}

public enum DeliveryStatus
{
    Pending = 1,

    Processing = 2,

    Delivered = 3,

    Read = 4,

    Acknowledged = 5,

    Failed = 6,

    Discarded = 7,

    Expired = 8,

    Retrying = 9
}