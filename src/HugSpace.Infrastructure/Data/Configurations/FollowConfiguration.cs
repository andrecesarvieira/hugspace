using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HugSpace.Domain.Entities;

namespace HugSpace.Infrastructure.Data.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("Follows");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FollowerId)
            .IsRequired();

        builder.Property(f => f.FollowingId)
            .IsRequired();

        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion<int>();

        // Indices para performance de queries sociais
        builder.HasIndex(f => f.FollowerId);
        builder.HasIndex(f => f.FollowingId);
        builder.HasIndex(f => new { f.FollowerId, f.FollowingId }).IsUnique();
        builder.HasIndex(f => new { f.FollowingId, f.Status });

        // Relationships
        builder.HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Constraint: não pode seguir a si mesmo
        builder.ToTable(t => t.HasCheckConstraint("CK_Follow_NotSelf", "\"FollowerId\" != \"FollowingId\""));
    }
}