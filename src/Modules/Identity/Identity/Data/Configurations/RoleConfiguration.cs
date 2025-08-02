using Shared.Data.Configurations;

namespace Identity.Data.Configurations;

public class RoleConfiguration : SpanishEntityConfiguration<Role, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Role> builder)
    {
        builder.Property(e => e.Id).HasColumnName("id_rol");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnName("nombre_rol");
        builder.Property(e => e.Description).HasMaxLength(255).HasColumnName("descripcion");

        builder.HasMany(r => r.Permissions).WithOne(p => p.Role).HasForeignKey(p => p.IdRole);
    }

    protected override string GetTableName() => "roles";
}
