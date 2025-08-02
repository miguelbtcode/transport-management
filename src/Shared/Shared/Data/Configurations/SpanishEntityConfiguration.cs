using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.DDD;

namespace Shared.Data.Configurations;

public abstract class SpanishEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);

        // Status available
        builder
            .Property(e => e.Enabled)
            .IsRequired()
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Audit fields automáticos en español
        builder.Property(e => e.CreatedAt).HasColumnName("fecha_creacion");
        builder.Property(e => e.CreatedBy).HasMaxLength(100).HasColumnName("creado_por");
        builder.Property(e => e.LastModified).HasColumnName("fecha_modificacion");
        builder.Property(e => e.LastModifiedBy).HasMaxLength(100).HasColumnName("modificado_por");

        // Logical deletion automático en español
        builder.Property(e => e.DeletedAt).HasColumnName("fecha_eliminacion");
        builder.Property(e => e.DeletedBy).HasMaxLength(100).HasColumnName("eliminado_por");
        builder.Property(e => e.DeletedReason).HasMaxLength(255).HasColumnName("razon_eliminacion");

        // Automatic indexes
        builder.HasIndex(e => e.Enabled).HasDatabaseName($"IX_{GetTableName()}_Activo");
        builder.HasIndex(e => e.DeletedAt).HasDatabaseName($"IX_{GetTableName()}_FechaEliminacion");
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName($"IX_{GetTableName()}_FechaCreacion");

        // Set table name
        builder.ToTable(GetTableName());

        // Call derived configuration
        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);

    protected virtual string GetTableName() => typeof(TEntity).Name.ToLower();
}
