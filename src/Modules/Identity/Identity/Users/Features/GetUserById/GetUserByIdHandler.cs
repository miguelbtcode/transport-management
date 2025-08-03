namespace Identity.Users.Features.GetUserById;

public record GetUserByIdQuery(Guid Id) : IQuery<Result<GetUserByIdResult>>;

public record GetUserByIdResult(UserDto UserDto);

internal class GetUserByIdHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByIdQuery, Result<GetUserByIdResult>>
{
    public async Task<Result<GetUserByIdResult>> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var user = await userRepository
            .Query(u => u.Id == query.Id, asNoTracking: true)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return UserErrors.NotFound(query.Id);

        var userDto = new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt!.Value,
            user.Enabled,
            user.UserRoles.Select(ur => ur.Role.Name).ToList()
        );

        return new GetUserByIdResult(userDto);
    }
}
