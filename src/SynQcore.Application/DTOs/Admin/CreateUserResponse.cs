namespace SynQcore.Application.DTOs.Admin;

public class CreateUserResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public Guid? UserId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }
}
