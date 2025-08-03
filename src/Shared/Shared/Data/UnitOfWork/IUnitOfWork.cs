using Shared.Data.Repository;

namespace Shared.Data.UnitOfWork;

public interface IUnitOfWork
{
    // Repository access
    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class;

    // Transaction management
    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default
    );
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default
    );

    // SaveChanges
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
