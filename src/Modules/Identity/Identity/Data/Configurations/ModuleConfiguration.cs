namespace Identity.Data.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("modules");

        // Properties
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnName("name");
        builder.Property(e => e.Description).HasMaxLength(255).HasColumnName("description");

        // Soft delete
        builder
            .Property(e => e.Enabled)
            .IsRequired()
            .HasColumnName("enabled")
            .HasDefaultValue(true);

        // Relationships
        builder.HasMany(m => m.Permissions).WithOne(p => p.Module).HasForeignKey(p => p.IdModule);
    }
}
