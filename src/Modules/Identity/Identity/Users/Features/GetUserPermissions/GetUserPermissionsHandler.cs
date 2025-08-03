using Identity.Permissions.Dtos;

namespace Identity.Users.Features.GetUserPermissions;

public record GetUserPermissionsQuery(Guid UserId) : IQuery<GetUserPermissionsResult>;

public record GetUserPermissionsResult(List<PermisoDto> Permissions);

internal class GetUserPermissionsHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserPermissionsQuery, GetUserPermissionsResult>
{
    public async Task<GetUserPermissionsResult> HandleAsync(
        GetUserPermissionsQuery query,
        CancellationToken cancellationToken
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var permissions = await userRepository
            .Query(u => u.Id == query.UserId, asNoTracking: true)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.Permissions)
            .Include(p => p.Role)
            .Include(p => p.Module)
            .Include(p => p.PermissionType)
            .Select(p => new PermisoDto(
                p.Id,
                p.Role.Name,
                p.Module.Name,
                p.PermissionType.Name,
                p.DateAssigned
            ))
            .ToListAsync(cancellationToken);

        return new GetUserPermissionsResult(permissions);
    }
}
