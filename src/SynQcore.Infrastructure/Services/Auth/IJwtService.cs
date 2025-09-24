using SynQcore.Infrastructure.Identity;

namespace SynQcore.Infrastructure.Services.Auth;

public interface IJwtService
{
    string GenerateToken(ApplicationUserEntity user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
}