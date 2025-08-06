using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Contracts.DDD;
using Shared.Contracts.Services;

namespace Shared.Data.Interceptors;

public class AuditableEntityInterceptor(ICurrentUserService currentUserService)
    : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
            return;

        var currentUser = _currentUserService.GetCurrentUserEmail() ?? "system";
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    HandleEntityAdded(entry, currentUser, utcNow);
                    break;
                case EntityState.Modified:
                    HandleEntityModified(entry, currentUser, utcNow);
                    break;
            }
        }
    }

    private static void HandleEntityAdded(EntityEntry entry, string currentUser, DateTime utcNow)
    {
        var entity = entry.Entity;
        var entityType = entity.GetType();

        SetPropertyIfExists(entityType, entity, "CreatedAt", utcNow);
        SetPropertyIfExists(entityType, entity, "CreatedBy", currentUser);
        SetPropertyIfExists(entityType, entity, "LastModified", utcNow);
        SetPropertyIfExists(entityType, entity, "LastModifiedBy", currentUser);
    }

    private static void HandleEntityModified(EntityEntry entry, string currentUser, DateTime utcNow)
    {
        var entity = entry.Entity;
        var entityType = entity.GetType();

        SetPropertyIfExists(entityType, entity, "LastModified", utcNow);
        SetPropertyIfExists(entityType, entity, "LastModifiedBy", currentUser);
    }

    private static void SetPropertyIfExists(
        Type entityType,
        object entity,
        string propertyName,
        object value
    )
    {
        var property = entityType.GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            if (IsTypeCompatible(property.PropertyType, value?.GetType()))
            {
                property.SetValue(entity, value);
            }
        }
    }

    private static bool IsTypeCompatible(Type propertyType, Type? valueType)
    {
        if (valueType == null)
            return !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) != null;
        return propertyType.IsAssignableFrom(valueType)
            || (
                Nullable.GetUnderlyingType(propertyType) != null
                && Nullable.GetUnderlyingType(propertyType)!.IsAssignableFrom(valueType)
            );
    }
}
