using Microsoft.AspNetCore.Identity;
using SynQcore.Domain.Entities.Organization;

namespace SynQcore.Infrastructure.Identity;

public class ApplicationUserEntity : IdentityUser<Guid>
{
    // Relacionamentos corporativos
    public Guid? EmployeeId { get; set; }

    // Navigation Properties
    public Employee? Employee { get; set; }
}
