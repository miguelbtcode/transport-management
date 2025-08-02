using Shared.Data.Configurations;

namespace Identity.Data.Configurations;

public class UserConfiguration : SpanishEntityConfiguration<User, Guid>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Id).HasColumnName("id_usuario");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("nombre");
        builder.Property(e => e.Email).IsRequired().HasMaxLength(150).HasColumnName("email");
        builder.Property(e => e.Password).IsRequired().HasMaxLength(255).HasColumnName("password");

        builder.HasIndex(e => e.Email).IsUnique();

        builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.IdUser);
    }

    protected override string GetTableName() => "usuarios";
}
