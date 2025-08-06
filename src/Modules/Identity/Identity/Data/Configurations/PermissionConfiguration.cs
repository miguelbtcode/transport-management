namespace Identity.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Pk
        builder.HasKey(e => e.Id);
        builder.ToTable("permissions");

        // Properties
        builder.Property(e => e.IdRole).IsRequired().HasColumnName("role_id");
        builder.Property(e => e.IdModule).IsRequired().HasColumnName("module_id");
        builder.Property(e => e.IdPermissionType).IsRequired().HasColumnName("permission_type_id");
        builder
            .Property(e => e.DateAssigned)
            .IsRequired()
            .HasColumnName("assigned_date")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

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
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.Property(e => e.DeletedBy).HasMaxLength(100).HasColumnName("deleted_by");
        builder.Property(e => e.DeletedReason).HasMaxLength(255).HasColumnName("deleted_reason");

        // Properties indexes
        builder
            .HasIndex(p => new
            {
                p.IdRole,
                p.IdModule,
                p.IdPermissionType,
            })
            .IsUnique()
            .HasDatabaseName("IX_Permissions_Unique_Active");

        // Audit indexes
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_permissions_CreatedAt");
        builder.HasIndex(e => e.DeletedAt).HasDatabaseName("IX_permissions_DeletedAt");
        builder.HasIndex(e => e.Enabled).HasDatabaseName("IX_permissions_Enabled");

        // Relationships
        builder
            .HasOne(p => p.Role)
            .WithMany(r => r.Permissions)
            .HasForeignKey(p => p.IdRole)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Module)
            .WithMany(m => m.Permissions)
            .HasForeignKey(p => p.IdModule)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.PermissionType)
            .WithMany(tp => tp.Permissions)
            .HasForeignKey(p => p.IdPermissionType)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
