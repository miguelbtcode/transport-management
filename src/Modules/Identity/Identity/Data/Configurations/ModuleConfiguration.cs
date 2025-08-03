using Shared.Data.Configurations;

namespace Identity.Data.Configurations;

public class ModuleConfiguration : SpanishEntityConfiguration<Module, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Module> builder)
    {
        // Override primary key column name
        builder.Property(e => e.Id).HasColumnName("id_modulo");

        // Entity-specific properties
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnName("nombre_modulo");
        builder.Property(e => e.Description).HasMaxLength(255).HasColumnName("descripcion");
        builder.HasMany(m => m.Permissions).WithOne(p => p.Module).HasForeignKey(p => p.IdModule);
    }

    protected override string GetTableName() => "modulos";
}
