using Identity.Modules.Dtos;
using Identity.Modules.Models;

namespace Identity.Modules.Features.GetModuleById;

public record GetModuleByIdQuery(Guid Id) : IQuery<Result<GetModuleByIdResult>>;

public record GetModuleByIdResult(ModuleDto Module);

internal class GetModuleByIdHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetModuleByIdQuery, Result<GetModuleByIdResult>>
{
    public async Task<Result<GetModuleByIdResult>> HandleAsync(
        GetModuleByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var moduleRepository = unitOfWork.Repository<Module>();

        var module = await moduleRepository.FirstOrDefaultAsync(
            m => m.Id == query.Id,
            asNoTracking: true,
            cancellationToken
        );

        if (module == null)
            return ModuleErrors.NotFound(query.Id);

        var moduleDto = new ModuleDto(module.Id, module.Name, module.Description, module.Enabled);

        return new GetModuleByIdResult(moduleDto);
    }
}
