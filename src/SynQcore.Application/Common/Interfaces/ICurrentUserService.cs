namespace SynQcore.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }

    string? UserName { get; }

    string? Role { get; }

    Guid? DepartmentId { get; }

    bool CanModerate { get; }

    bool IsAdmin { get; }
}
