using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("CREDENCIAL");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.UserId)
            .HasColumnName("USUARIO_ID")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("TIPO")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("ESTADO")
            .IsRequired();

        builder.Property(x => x.SecretHash)
            .HasColumnName("HASH_SECRETO")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Algorithm)
            .HasColumnName("ALGORITMO")
            .HasMaxLength(100);

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("FECHA_EXPIRACION");

        builder.Property(x => x.LastUsedAt)
            .HasColumnName("ULTIMO_USO");

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
            .HasDatabaseName("IX_CREDENCIAL_USUARIO_ID");

        builder.HasOne(x => x.User)
            .WithMany(x => x.Credentials)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CREDENCIAL_USUARIO");
    }
}