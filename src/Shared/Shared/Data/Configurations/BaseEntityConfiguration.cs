using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.DDD;

namespace Shared.Data.Configurations;

public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);

        // Audit fields - Campos base de auditoría
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");

        builder.Property(e => e.CreatedBy).HasMaxLength(100).HasColumnName("created_by");

        builder.Property(e => e.LastModified).HasColumnName("last_modified");

        builder.Property(e => e.LastModifiedBy).HasMaxLength(100).HasColumnName("last_modified_by");

        // Logical deletion fields - Campos de eliminación lógica
        builder
            .Property(e => e.Enabled)
            .IsRequired()
            .HasColumnName("enabled")
            .HasDefaultValue(true);

        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");

        builder.Property(e => e.DeletedBy).HasMaxLength(100).HasColumnName("deleted_by");

        builder.Property(e => e.DeletedReason).HasMaxLength(255).HasColumnName("deleted_reason");

        // Common indexes - Índices comunes
        builder.HasIndex(e => e.Enabled).HasDatabaseName($"IX_{GetTableName()}_Enabled");

        builder.HasIndex(e => e.DeletedAt).HasDatabaseName($"IX_{GetTableName()}_DeletedAt");

        builder.HasIndex(e => e.CreatedAt).HasDatabaseName($"IX_{GetTableName()}_CreatedAt");

        // Set table name
        builder.ToTable(GetTableName());

        // Call derived configuration
        ConfigureEntity(builder);
    }

    // Método abstracto para configuración específica de cada entidad
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);

    // Método virtual para obtener el nombre de tabla (puede ser sobrescrito)
    protected virtual string GetTableName() => typeof(TEntity).Name.ToLower();
}
