using System.Runtime.InteropServices;
using HugSpace.Domain.Common;
using Microsoft.Extensions.Options;

namespace HugSpace.Domain.Entities;

public class User : BaseEntity
{
    public string? Username { get; private set; }
    public string? Email { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Bio { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public bool IsVerified { get; private set; }
    public bool IsPrivate { get; private set; }

    // Propriedade para navagação (EF Core)
    public ICollection<Post> Posts { get; private set; } = [];
    public ICollection<Follow> Following { get; private set; } = [];
    public ICollection<Follow> Followers { get; private set; } = [];

    private User(){}

    public User(string username, string email, string displayName)
    {
        Username = username;
        Email = email;
        DisplayName = displayName;
        IsVerified = false;
        IsPrivate = false;
    }

    public void UpdateProfile(string displayName, string bio)
    {
        DisplayName = displayName;
        Bio = bio;
        UpdateTimestamp();
    }

    public void SetProfileImage(string imageUrl)
    {
        ProfileImageUrl = imageUrl;
        UpdateTimestamp();
    }

    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
        UpdateTimestamp();
    }
}
