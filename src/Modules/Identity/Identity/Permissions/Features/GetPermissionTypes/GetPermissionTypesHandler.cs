using Identity.Permissions.Dtos;

namespace Identity.Permissions.Features.GetPermissionTypes;

public record GetPermissionTypesQuery() : IQuery<GetPermissionTypesResult>;

public record GetPermissionTypesResult(List<PermissionTypeDto> PermissionTypes);

internal class GetPermissionTypesHandler(IdentityDbContext dbContext)
    : IQueryHandler<GetPermissionTypesQuery, GetPermissionTypesResult>
{
    public async Task<GetPermissionTypesResult> HandleAsync(
        GetPermissionTypesQuery query,
        CancellationToken cancellationToken
    )
    {
        var permissionTypes = await dbContext
            .PermissionTypes.AsNoTracking()
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
