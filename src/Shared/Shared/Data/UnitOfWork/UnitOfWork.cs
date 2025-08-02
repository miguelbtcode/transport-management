using Microsoft.EntityFrameworkCore;

namespace Shared.Data.UnitOfWork;

public class UnitOfWork<TContext>(TContext dbContext) : IUnitOfWork
    where TContext : DbContext
{
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default
    )
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync(
                cancellationToken
            );

            try
            {
                var result = await operation();
                await dbContext.SaveChangesAsync(cancellationToken);
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
                return true; // Dummy return for generic method
            },
            cancellationToken
        );
    }
}
