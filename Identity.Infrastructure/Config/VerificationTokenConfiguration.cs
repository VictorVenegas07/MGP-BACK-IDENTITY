using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class VerificationTokenConfiguration : IEntityTypeConfiguration<VerificationToken>
{
    public void Configure(EntityTypeBuilder<VerificationToken> builder)
    {
        builder.ToTable("TOKEN_VERIFICACION");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.UserId)
            .HasColumnName("USUARIO_ID")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("TIPO")
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasColumnName("HASH_TOKEN")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("FECHA_EXPIRACION")
            .IsRequired();

        builder.Property(x => x.UsedAt)
            .HasColumnName("FECHA_USO");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("FECHA_CREACION")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("FECHA_ACTUALIZACION");

        builder.Property(x => x.IsDeleted)
            .HasColumnName("ES_ELIMINADO")
            .IsRequired();

        builder.Property(x => x.DeletedOn)
            .HasColumnName("FECHA_ELIMINACION");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_TOKEN_VERIFICACION_USUARIO_ID");

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_TOKEN_VERIFICACION_HASH_TOKEN");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_TOKEN_VERIFICACION_FECHA_EXPIRACION");

        builder.HasOne(x => x.User)
            .WithMany(x => x.VerificationTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TOKEN_VERIFICACION_USUARIO");
    }
}