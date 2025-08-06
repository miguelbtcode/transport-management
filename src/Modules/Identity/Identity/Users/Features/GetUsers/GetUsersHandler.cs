using Identity.Roles.Dtos;

namespace Identity.Users.Features.GetUsers;

public record GetUsersQuery(PaginationRequest PaginationRequest, bool? Enabled = null)
    : IQuery<GetUsersResult>;

public record GetUsersResult(PaginatedResult<UserDto> Users);

internal class GetUsersHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    public async Task<GetUsersResult> HandleAsync(
        GetUsersQuery query,
        CancellationToken cancellationToken
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var totalCount = await userRepository.CountAsync(cancellationToken: cancellationToken);

        var usersQuery = userRepository.Query(asNoTracking: true);

        // Apply filters
        if (query.Enabled.HasValue)
            usersQuery = usersQuery.Where(u => u.Enabled == query.Enabled.Value);

        // Apply includes
        usersQuery = usersQuery.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);

        var users = await usersQuery
            .OrderBy(u => u.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userDtos = users
            .Select(u => new UserDto(
                u.Id,
                u.Name,
                u.Email,
                u.CreatedAt,
                u.Enabled,
                u.UserRoles.Select(ur => new RoleDto(
                        ur.Role.Id,
                        ur.Role.Name,
                        ur.Role.Description,
                        ur.Role.Enabled
                    ))
                    .ToList()
            ))
            .ToList();

        return new GetUsersResult(
            new PaginatedResult<UserDto>(pageIndex, pageSize, totalCount, userDtos)
        );
    }
}
