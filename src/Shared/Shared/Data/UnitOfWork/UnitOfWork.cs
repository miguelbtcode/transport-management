using Microsoft.EntityFrameworkCore;
using Shared.Data.Repository;

namespace Shared.Data.UnitOfWork;

public class UnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<TEntity>(_context);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default
    )
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(
                cancellationToken
            );

            try
            {
                var result = await operation();
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    )
    {
        await ExecuteInTransactionAsync(
            async () =>
            {
                await operation();
                return true;
            },
            cancellationToken
        );
    }
}
