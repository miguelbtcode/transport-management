using Shared.Pagination;

namespace Identity.Users.Features.GetUsers;

public record GetUsersQuery(PaginationRequest PaginationRequest) : IQuery<GetUsersResult>;

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

        var users = await userRepository
            .Query(asNoTracking: true)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userDtos = users
            .Select(u => new UserDto(
                u.Id,
                u.Name,
                u.Email,
                u.CreatedAt!.Value,
                u.Enabled,
                u.UserRoles.Select(ur => ur.Role.Name).ToList()
            ))
            .ToList();

        return new GetUsersResult(
            new PaginatedResult<UserDto>(pageIndex, pageSize, totalCount, userDtos)
        );
    }
}
