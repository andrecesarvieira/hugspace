using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Infrastructure.Data.Configurations.Communication;

public class UserInterestConfiguration : IEntityTypeConfiguration<UserInterest>
{
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        // Tabela
        builder.ToTable("UserInterests");

        // Chave primária
        builder.HasKey(ui => ui.Id);

        // Propriedades obrigatórias
        builder.Property(ui => ui.UserId)
            .IsRequired();

        builder.Property(ui => ui.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ui => ui.InterestValue)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ui => ui.Score)
            .IsRequired()
            .HasPrecision(4, 2) // 0.00 to 10.00
            .HasDefaultValue(1.0);

        builder.Property(ui => ui.InteractionCount)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(ui => ui.LastInteractionAt)
            .IsRequired();

        builder.Property(ui => ui.Source)
            .IsRequired()
            .HasConversion<int>();

        // Relacionamentos
        builder.HasOne(ui => ui.User)
            .WithMany()
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Constraint único: um usuário pode ter apenas um interesse por tipo/valor
        builder.HasIndex(ui => new { ui.UserId, ui.Type, ui.InterestValue })
            .IsUnique()
            .HasDatabaseName("IX_UserInterests_UserId_Type_Value_Unique");

        // Índice para consultas do algoritmo de recomendação
        builder.HasIndex(ui => new { ui.UserId, ui.Score })
            .HasDatabaseName("IX_UserInterests_UserId_Score")
            .IsDescending(false, true);

        // Índice para busca por tipo de interesse
        builder.HasIndex(ui => new { ui.Type, ui.InterestValue, ui.Score })
            .HasDatabaseName("IX_UserInterests_Type_Value_Score")
            .IsDescending(false, false, true);

        // Índice para limpeza de interesses antigos
        builder.HasIndex(ui => new { ui.LastInteractionAt, ui.Score })
            .HasDatabaseName("IX_UserInterests_LastInteraction_Score");
    }
}
