using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Infrastructure.Data.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.LikeCount)
            .HasDefaultValue(0);

        builder.Property(p => p.CommentCount)
            .HasDefaultValue(0);

        builder.Property(p => p.ViewCount)
            .HasDefaultValue(0);

        // Indices para performance de feeds
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => new { p.UserId, p.CreatedAt });
        builder.HasIndex(p => new { p.IsPublic, p.CreatedAt });

        // Relationship
        builder.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}