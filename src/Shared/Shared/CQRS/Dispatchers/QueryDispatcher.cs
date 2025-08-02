using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.CQRS;

namespace Shared.CQRS.Dispatchers;

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResponse> DispatchAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken = default
    )
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(
            query.GetType(),
            typeof(TResponse)
        );
        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(
            nameof(IQueryHandler<IQuery<TResponse>, TResponse>.HandleAsync)
        );
        var task = (Task<TResponse>)method!.Invoke(handler, [query, cancellationToken])!;

        return await task;
    }
}
