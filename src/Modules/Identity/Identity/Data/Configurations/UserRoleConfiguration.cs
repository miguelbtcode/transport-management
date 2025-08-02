using Shared.Data.Configurations;

namespace Identity.Data.Configurations;

public class UserRoleConfiguration : SpanishEntityConfiguration<UserRole, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
    {
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.IdUser).IsRequired().HasColumnName("id_usuario");
        builder.Property(e => e.IdRole).IsRequired().HasColumnName("id_rol");
        builder
            .Property(e => e.DateAssigned)
            .IsRequired()
            .HasColumnName("fecha_asignacion")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.IdUser)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.IdRole)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ur => new { ur.IdUser, ur.IdRole }).IsUnique();
    }

    protected override string GetTableName() => "usuarios_roles";
}
