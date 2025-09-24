using Microsoft.AspNetCore.Identity;
using SynQcore.Domain.Entities.Organization;
using SynQcore.Domain.Identity;

namespace SynQcore.Infrastructure.Identity;

public class ApplicationUserEntity : IdentityUser<Guid>
{
    // Propriedades adicionais do Domain
    public Guid? EmployeeId { get; set; }

    // Propriedades de Navegação
    public Employee? Employee { get; set; }

    //Metodo para mapear o Domain
    public static ApplicationUserEntity FromDomain(ApplicationUser domainUser)
    {
        return new ApplicationUserEntity
        {
            Id = domainUser.Id,
            UserName = domainUser.UserName,
            Email = domainUser.Email,
            EmailConfirmed = domainUser.EmailConfirmed,
            PasswordHash = domainUser.PasswordHash,
            PhoneNumber = domainUser.PhoneNumber,
            PhoneNumberConfirmed = domainUser.PhoneNumberConfirmed,
            TwoFactorEnabled = domainUser.TwoFactorEnabled,
            LockoutEnd = domainUser.LockoutEnd,
            LockoutEnabled = domainUser.LockoutEnabled,
            AccessFailedCount = domainUser.AccessFailedCount,
            EmployeeId = domainUser.EmployeeId
        };
    }
}