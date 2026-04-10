using Identity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Config.Persistence;

public class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        builder.ToTable("MEMBRESIA_TENANT");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.UserId)
            .HasColumnName("USUARIO_ID")
            .IsRequired();

        builder.Property(x => x.TenantId)
            .HasColumnName("TENANT_ID")
            .IsRequired();

        builder.Property(x => x.RoleCode)
            .HasColumnName("CODIGO_ROL")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("ESTADO")
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .HasColumnName("FECHA_VINCULACION")
            .IsRequired();

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
            .HasDatabaseName("IX_MEMBRESIA_TENANT_USUARIO_ID");

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("IX_MEMBRESIA_TENANT_TENANT_ID");

        builder.HasIndex(x => new { x.UserId, x.TenantId })
            .IsUnique()
            .HasDatabaseName("IX_MEMBRESIA_TENANT_USUARIO_ID_TENANT_ID");

        builder.HasOne(x => x.User)
            .WithMany(x => x.TenantMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_MEMBRESIA_TENANT_USUARIO");
    }
}