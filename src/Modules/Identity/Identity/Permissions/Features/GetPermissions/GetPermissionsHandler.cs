using Identity.Permissions.Dtos;

namespace Identity.Permissions.Features.GetPermissions;

public record GetPermissionsQuery(
    Guid? RoleId = null,
    Guid? ModuleId = null,
    Guid? PermissionTypeId = null,
    bool? Enabled = null
) : IQuery<GetPermissionsResult>;

public record GetPermissionsResult(List<PermissionDetailDto> Permissions);

internal class GetPermissionsHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetPermissionsQuery, GetPermissionsResult>
{
    public async Task<GetPermissionsResult> HandleAsync(
        GetPermissionsQuery query,
        CancellationToken cancellationToken
    )
    {
        var permissionRepository = unitOfWork.Repository<Permission>();

        var permissionsQuery = permissionRepository.Query(asNoTracking: true);

        // Aplicar filtros
        if (query.RoleId.HasValue)
            permissionsQuery = permissionsQuery.Where(p => p.IdRole == query.RoleId.Value);

        if (query.ModuleId.HasValue)
            permissionsQuery = permissionsQuery.Where(p => p.IdModule == query.ModuleId.Value);

        if (query.PermissionTypeId.HasValue)
            permissionsQuery = permissionsQuery.Where(p =>
                p.IdPermissionType == query.PermissionTypeId.Value
            );

        if (query.Enabled.HasValue)
            permissionsQuery = permissionsQuery.Where(p => p.Enabled == query.Enabled.Value);

        // Aplicar includes
        permissionsQuery = permissionsQuery
            .Include(p => p.Role)
            .Include(p => p.Module)
            .Include(p => p.PermissionType);

        var permissions = await permissionsQuery
            .OrderBy(p => p.Role.Name)
            .ThenBy(p => p.Module.Name)
            .ThenBy(p => p.PermissionType.Name)
            .Select(p => new PermissionDetailDto(
                p.Id,
                p.IdRole,
                p.Role.Name,
                p.IdModule,
                p.Module.Name,
                p.IdPermissionType,
                p.PermissionType.Name,
                p.PermissionType.Code,
                p.DateAssigned,
                p.Enabled
            ))
            .ToListAsync(cancellationToken);

        return new GetPermissionsResult(permissions);
    }
}
