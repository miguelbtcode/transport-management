namespace Identity.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("users");

        // Properties
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
        builder.Property(e => e.Email).IsRequired().HasMaxLength(150).HasColumnName("email");
        builder.Property(e => e.Password).IsRequired().HasMaxLength(255).HasColumnName("password");

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
        builder.HasIndex(e => e.Email).IsUnique();

        // Audit indexes
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_users_CreatedAt");
        builder.HasIndex(e => e.DeletedAt).HasDatabaseName("IX_users_DeletedAt");
        builder.HasIndex(e => e.Enabled).HasDatabaseName("IX_users_Enabled");

        // Relationships
        builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.IdUser);
    }
}
