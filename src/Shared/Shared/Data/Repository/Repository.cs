namespace Shared.Data.Repository;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        if (asNoTracking)
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);

        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    )
    {
        return predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default
    )
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default
    )
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual IQueryable<TEntity> Query(bool asNoTracking = false)
    {
        return asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsQueryable();
    }

    public virtual IQueryable<TEntity> Query(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false
    )
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        return query.Where(predicate);
    }
}
