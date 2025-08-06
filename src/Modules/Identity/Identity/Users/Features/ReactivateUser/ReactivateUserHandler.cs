namespace Identity.Users.Features.ReactivateUser;

public record ReactivateUserCommand(Guid UserId) : ICommand<Result<ReactivateUserResult>>;

public record ReactivateUserResult(bool IsSuccess, string Message);

public class ReactivateUserCommandValidator : AbstractValidator<ReactivateUserCommand>
{
    public ReactivateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId es requerido");
    }
}

internal class ReactivateUserHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<ReactivateUserCommand, Result<ReactivateUserResult>>
{
    public async Task<Result<ReactivateUserResult>> HandleAsync(
        ReactivateUserCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var user = await userRepository.FirstOrDefaultAsync(
            u => u.Id == command.UserId,
            asNoTracking: false,
            cancellationToken
        );

        if (user == null)
            return UserErrors.NotFound(command.UserId);

        if (user.Enabled)
            return UserErrors.AlreadyActive;

        user.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new ReactivateUserResult(true, "Usuario reactivado exitosamente");
    }
}
