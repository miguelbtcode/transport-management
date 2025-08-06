namespace Identity.Users.Features.CreateUser;

public record CreateUserCommand(CreateUserDto User) : ICommand<Result<CreateUserResult>>;

public record CreateUserResult(Guid Id);

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.User.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.User.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.User.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.User.RoleIds).NotEmpty();
    }
}

internal class CreateUserHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, Result<CreateUserResult>>
{
    public async Task<Result<CreateUserResult>> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var existingUser = await userRepository.FirstOrDefaultAsync(
            u => u.Email == command.User.Email.ToLower(),
            asNoTracking: true,
            cancellationToken
        );

        if (existingUser != null)
            return UserErrors.EmailAlreadyExists(command.User.Email);

        var userId = await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var hashedPassword = passwordHasher.HashPassword(command.User.Password);
                var usuario = User.Create(
                    command.User.Name,
                    command.User.Email,
                    hashedPassword,
                    command.User.Enabled
                );

                foreach (var roleId in command.User.RoleIds)
                    usuario.AssignRole(roleId);

                await userRepository.AddAsync(usuario, cancellationToken);
                return usuario.Id;
            },
            cancellationToken
        );

        return new CreateUserResult(userId);
    }
}
