using Identity.Permissions.Dtos;
using Identity.PermissionTypes.Models;

namespace Identity.PermissionTypes.Features.GetPermissionTypes;

public record GetPermissionTypesQuery() : IQuery<GetPermissionTypesResult>;

public record GetPermissionTypesResult(List<PermissionTypeDto> PermissionTypes);

internal class GetPermissionTypesHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetPermissionTypesQuery, GetPermissionTypesResult>
{
    public async Task<GetPermissionTypesResult> HandleAsync(
        GetPermissionTypesQuery query,
        CancellationToken cancellationToken
    )
    {
        var permissionTypeRepository = unitOfWork.Repository<PermissionType>();

        var permissionTypes = await permissionTypeRepository
            .Query(asNoTracking: true)
            .OrderBy(pt => pt.Category)
            .ThenBy(pt => pt.Name)
            .Select(pt => new PermissionTypeDto(
                pt.Id,
                pt.Name,
                pt.Code,
                pt.Category,
                pt.Description
            ))
            .ToListAsync(cancellationToken);

        return new GetPermissionTypesResult(permissionTypes);
    }
}
