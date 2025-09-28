namespace SynQcore.Domain.Entities;

public enum DocumentType
{
    Policy = 1,

    Procedure = 2,

    Manual = 3,

    Form = 4,

    Template = 5,

    Presentation = 6,

    Spreadsheet = 7,

    Contract = 8,

    Certificate = 9,

    Image = 10,

    Video = 11,

    Audio = 12,

    Archive = 13,

    General = 14
}

public enum DocumentStatus
{
    Draft = 1,

    PendingReview = 2,

    InReview = 3,

    Approved = 4,

    Rejected = 5,

    Archived = 6,

    Expired = 7,

    Suspended = 8,

    Obsolete = 9
}

public enum DocumentAccessLevel
{
    Public = 1,

    Internal = 2,

    Departmental = 3,

    Restricted = 4,

    Confidential = 5,

    Secret = 6
}

public enum DocumentCategory
{
    HumanResources = 1,

    Financial = 2,

    Legal = 3,

    Technology = 4,

    Operations = 5,

    Marketing = 6,

    Quality = 7,

    Security = 8,

    Training = 9,

    Projects = 10,

    Administrative = 11,

    Executive = 12
}

public enum AccessType
{
    Read = 1,

    Download = 2,

    Comment = 3,

    Edit = 4,

    FullControl = 5
}

public enum DocumentAction
{
    View = 1,

    Download = 2,

    Upload = 3,

    Edit = 4,

    Share = 5,

    Delete = 6,

    Archive = 7,

    Approve = 8,

    Reject = 9,

    Comment = 10,

    AccessDenied = 11
}

public enum MediaAssetType
{
    Image = 1,

    Video = 2,

    Audio = 3,

    Logo = 4,

    Icon = 5,

    Banner = 6,

    Avatar = 7,

    Screenshot = 8,

    Diagram = 9,

    Scan = 10
}

public enum MediaAssetCategory
{
    Branding = 1,

    Marketing = 2,

    HumanResources = 3,

    Training = 4,

    Events = 5,

    Products = 6,

    Facilities = 7,

    Team = 8,

    Projects = 9,

    Technical = 10,

    Templates = 11,

    General = 12
}