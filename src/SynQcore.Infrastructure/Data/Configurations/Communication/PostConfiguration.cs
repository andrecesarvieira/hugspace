using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts", "Communication");

        // Propriedades
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(50000); // 50KB para knowledge articles

        builder.Property(e => e.Summary)
            .HasMaxLength(1000);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(2000);

        builder.Property(e => e.DocumentUrl)
            .HasMaxLength(2000);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(PostType.Post);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(PostStatus.Draft)
            .HasSentinel(PostStatus.Draft);

        builder.Property(e => e.Visibility)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(PostVisibility.Public)
            .HasSentinel(PostVisibility.Public);

        builder.Property(e => e.Version)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("1.0");

        builder.Property(e => e.ViewCount)
            .HasDefaultValue(0);

        builder.Property(e => e.LikeCount)
            .HasDefaultValue(0);

        builder.Property(e => e.CommentCount)
            .HasDefaultValue(0);

        // Índices
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Visibility);
        builder.HasIndex(e => e.AuthorId);
        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.DepartmentId);
        builder.HasIndex(e => e.TeamId);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.ViewCount);
        builder.HasIndex(e => new { e.Type, e.Status, e.Visibility });

        // Relacionamentos
        builder.HasOne(e => e.Author)
            .WithMany(a => a.Posts)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Posts)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Team)
            .WithMany(t => t.Posts)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento de versionamento (self-referencing)
        builder.HasOne(e => e.ParentPost)
            .WithMany(e => e.Versions)
            .HasForeignKey(e => e.ParentPostId)
            .OnDelete(DeleteBehavior.Restrict);

        // Query Filter para soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}