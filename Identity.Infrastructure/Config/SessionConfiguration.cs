using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("SESION");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.UserId)
            .HasColumnName("USUARIO_ID")
            .IsRequired();

        builder.Property(x => x.TenantId)
            .HasColumnName("TENANT_ID");

        builder.Property(x => x.AuthMethod)
            .HasColumnName("METODO_AUTENTICACION")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasColumnName("DIRECCION_IP")
            .HasMaxLength(64);

        builder.Property(x => x.UserAgent)
            .HasColumnName("USER_AGENT")
            .HasMaxLength(1000);

        builder.Property(x => x.DeviceName)
            .HasColumnName("NOMBRE_DISPOSITIVO")
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasColumnName("ESTADO")
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .HasColumnName("FECHA_INICIO")
            .IsRequired();

        builder.Property(x => x.LastSeenAt)
            .HasColumnName("ULTIMA_ACTIVIDAD");

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("FECHA_EXPIRACION")
            .IsRequired();

        builder.Property(x => x.RevokedAt)
            .HasColumnName("FECHA_REVOCACION");

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
            .HasDatabaseName("IX_SESION_USUARIO_ID");

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("IX_SESION_TENANT_ID");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_SESION_ESTADO");

        builder.HasIndex(x => x.ExpiresAt)
            .HasDatabaseName("IX_SESION_FECHA_EXPIRACION");

        builder.HasOne(x => x.User)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_SESION_USUARIO");
    }
}