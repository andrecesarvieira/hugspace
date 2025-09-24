using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SynQcore.Infrastructure.Identity;

namespace SynQcore.Infrastructure.Data;

public class SynQcoreIdentityDbContext : IdentityDbContext<ApplicationUserEntity, IdentityRole<Guid>, Guid>
{
    public SynQcoreIdentityDbContext(DbContextOptions<SynQcoreIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Rename Identity tables to avoid conflicts
        builder.Entity<ApplicationUserEntity>().ToTable("AspNetUsers");
        builder.Entity<IdentityRole<Guid>>().ToTable("AspNetRoles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens");
    }
}