namespace Identity.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("roles");

        // Properties
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50).HasColumnName("name");
        builder.Property(e => e.Description).HasMaxLength(255).HasColumnName("description");

        // Soft delete
        builder
            .Property(e => e.Enabled)
            .IsRequired()
            .HasColumnName("enabled")
            .HasDefaultValue(true);

        // Audit fields properties
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.LastModified).HasColumnName("last_modified");
        builder.Property(e => e.CreatedBy).HasMaxLength(100).HasColumnName("created_by");
        builder.Property(e => e.LastModifiedBy).HasMaxLength(100).HasColumnName("last_modified_by");

        // Audit indexes
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_roles_CreatedAt");
        builder.HasIndex(e => e.Enabled).HasDatabaseName("IX_roles_Enabled");

        // Relationships
        builder.HasMany(r => r.Permissions).WithOne(p => p.Role).HasForeignKey(p => p.IdRole);
    }
}
