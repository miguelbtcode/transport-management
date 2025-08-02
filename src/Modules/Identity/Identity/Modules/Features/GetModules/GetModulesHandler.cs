using Identity.Modules.Dtos;

namespace Identity.Modules.Features.GetModules;

public record GetModulesQuery() : IQuery<GetModulesResult>;

public record GetModulesResult(List<ModuleDto> Modules);

internal class GetModulesHandler(IdentityDbContext dbContext)
    : IQueryHandler<GetModulesQuery, GetModulesResult>
{
    public async Task<GetModulesResult> HandleAsync(
        GetModulesQuery query,
        CancellationToken cancellationToken
    )
    {
        var modules = await dbContext
            .Modules.AsNoTracking()
            .Where(m => m.Enabled)
            .OrderBy(m => m.Name)
            .Select(m => new ModuleDto(m.Id, m.Name, m.Description, m.Enabled))
            .ToListAsync(cancellationToken);

        return new GetModulesResult(modules);
    }
}
