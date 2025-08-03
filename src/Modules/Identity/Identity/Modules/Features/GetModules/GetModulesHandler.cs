using Identity.Modules.Dtos;

namespace Identity.Modules.Features.GetModules;

public record GetModulesQuery() : IQuery<GetModulesResult>;

public record GetModulesResult(List<ModuleDto> Modules);

internal class GetModulesHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetModulesQuery, GetModulesResult>
{
    public async Task<GetModulesResult> HandleAsync(
        GetModulesQuery query,
        CancellationToken cancellationToken
    )
    {
        var moduleRepository = unitOfWork.Repository<Module>();

        var modules = await moduleRepository
            .Query(m => m.Enabled, asNoTracking: true)
            .OrderBy(m => m.Name)
            .Select(m => new ModuleDto(m.Id, m.Name, m.Description, m.Enabled))
            .ToListAsync(cancellationToken);

        return new GetModulesResult(modules);
    }
}
