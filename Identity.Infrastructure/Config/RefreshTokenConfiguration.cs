using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("TOKEN_REFRESH");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.SessionId)
            .HasColumnName("SESION_ID")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("USUARIO_ID")
            .IsRequired();

        builder.Property(x => x.TenantId)
            .HasColumnName("TENANT_ID");

        builder.Property(x => x.TokenHash)
            .HasColumnName("HASH_TOKEN")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.IssuedAt)
            .HasColumnName("FECHA_EMISION")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("FECHA_EXPIRACION")
            .IsRequired();

        builder.Property(x => x.RevokedAt)
            .HasColumnName("FECHA_REVOCACION");

        builder.Property(x => x.ReuseDetectedAt)
            .HasColumnName("FECHA_REUSO_DETECTADO");

        builder.Property(x => x.RotatedFromTokenId)
            .HasColumnName("TOKEN_ROTADO_DESDE_ID");

        builder.Property(x => x.ReplacedByTokenId)
            .HasColumnName("REEMPLAZADO_POR_TOKEN_ID");

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

        builder.HasIndex(x => x.SessionId)
            .HasDatabaseName("IX_TOKEN_REFRESH_SESION_ID");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_TOKEN_REFRESH_USUARIO_ID");

        builder.HasIndex(x => x.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_TOKEN_REFRESH_HASH_TOKEN");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_TOKEN_REFRESH_FECHA_EXPIRACION");

        builder.HasOne(x => x.Session)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TOKEN_REFRESH_SESION");

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TOKEN_REFRESH_USUARIO");

        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(x => x.RotatedFromTokenId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TOKEN_REFRESH_ROTADO_DESDE");

        builder.HasOne<RefreshToken>()
            .WithMany()
            .HasForeignKey(x => x.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TOKEN_REFRESH_REEMPLAZADO_POR");
    }
}