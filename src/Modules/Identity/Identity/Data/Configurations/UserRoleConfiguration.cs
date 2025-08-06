namespace Identity.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("user_roles");

        // Properties
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.IdUser).IsRequired().HasColumnName("user_id");
        builder.Property(e => e.IdRole).IsRequired().HasColumnName("role_id");

        // Relationships
        builder
            .Property(e => e.DateAssigned)
            .IsRequired()
            .HasColumnName("date_assigned")
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
}
