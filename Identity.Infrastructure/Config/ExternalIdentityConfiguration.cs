using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
    public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
    {
        builder.ToTable("IDENTIDAD_EXTERNA");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.UserId)
            .HasColumnName("USUARIO_ID")
            .IsRequired();

        builder.Property(x => x.ProviderType)
            .HasColumnName("TIPO_PROVEEDOR")
            .IsRequired();

        builder.Property(x => x.ProviderSubject)
            .HasColumnName("SUJETO_PROVEEDOR")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ProviderEmail)
            .HasColumnName("CORREO_PROVEEDOR")
            .HasMaxLength(320);

        builder.Property(x => x.ProviderEmailVerified)
            .HasColumnName("CORREO_PROVEEDOR_VERIFICADO")
            .IsRequired();

        builder.Property(x => x.LinkedAt)
            .HasColumnName("FECHA_VINCULACION")
            .IsRequired();

        builder.Property(x => x.LastLoginAt)
            .HasColumnName("ULTIMO_INICIO_SESION");

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

        builder.HasIndex(x => new { x.ProviderType, x.ProviderSubject })
            .IsUnique()
            .HasDatabaseName("IX_IDENTIDAD_EXTERNA_TIPO_PROVEEDOR_SUJETO_PROVEEDOR");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_IDENTIDAD_EXTERNA_USUARIO_ID");

        builder.HasOne(x => x.User)
            .WithMany(x => x.ExternalIdentities)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_IDENTIDAD_EXTERNA_USUARIO");
    }
}