using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        // Indices
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        // Relationships
        builder.HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Following)
            .WithOne(f => f.Follower)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Followers)
            .WithOne(f => f.Following)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}