using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USUARIO");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.PrimaryEmail)
            .HasColumnName("CORREO_PRINCIPAL")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.EmailVerified)
            .HasColumnName("CORREO_VERIFICADO")
            .IsRequired();

        builder.Property(x => x.UserName)
            .HasColumnName("NOMBRE_USUARIO")
            .HasMaxLength(100);

        builder.Property(x => x.DisplayName)
            .HasColumnName("NOMBRE_MOSTRAR")
            .HasMaxLength(150);

        builder.Property(x => x.AvatarUrl)
            .HasColumnName("URL_AVATAR")
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasColumnName("ESTADO")
            .IsRequired();

        builder.Property(x => x.IsPlatformAdmin)
            .HasColumnName("ES_ADMINISTRADOR_PLATAFORMA")
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

        builder.HasIndex(x => x.PrimaryEmail)
            .IsUnique()
            .HasDatabaseName("IX_USUARIO_CORREO_PRINCIPAL");

        builder.HasIndex(x => x.UserName)
            .IsUnique()
            .HasDatabaseName("IX_USUARIO_NOMBRE_USUARIO");
    }
}