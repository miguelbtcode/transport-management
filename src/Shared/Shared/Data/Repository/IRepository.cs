namespace Shared.Data.Repository;

public interface IRepository<TEntity>
    where TEntity : class
{
    // Queries
    Task<TEntity?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<TEntity>> GetAllAsync(
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    );
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    );
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    // Commands
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    );
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);

    // Query building
    IQueryable<TEntity> Query(bool asNoTracking = false);
    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
}
