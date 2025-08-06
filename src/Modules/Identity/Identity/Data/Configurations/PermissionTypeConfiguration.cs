namespace Identity.Data.Configurations;

public class PermissionTypeConfiguration : IEntityTypeConfiguration<PermissionType>
{
    public void Configure(EntityTypeBuilder<PermissionType> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("permission_types");

        // Properties
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(30).HasColumnName("name");
        builder.Property(e => e.Code).IsRequired().HasMaxLength(20).HasColumnName("code");
        builder.Property(e => e.Category).IsRequired().HasMaxLength(20).HasColumnName("category");
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

        // Properties indexes
        builder.HasIndex(e => e.Code).IsUnique();

        // Audit indexes
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_permission_types_CreatedAt");
        builder.HasIndex(e => e.Enabled).HasDatabaseName("IX_permission_types_Enabled");

        // Relationships
        builder
            .HasMany(tp => tp.Permissions)
            .WithOne(p => p.PermissionType)
            .HasForeignKey(p => p.IdPermissionType);
    }
}
