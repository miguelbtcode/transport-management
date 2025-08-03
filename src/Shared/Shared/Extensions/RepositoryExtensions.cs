using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Repository;
using Shared.Data.UnitOfWork;

namespace Shared.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryPattern<TContext>(
        this IServiceCollection services
    )
        where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}
