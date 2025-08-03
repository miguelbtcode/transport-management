namespace Identity.Users.Features.DeleteUser;

public record DeleteUserCommand(Guid UserId) : ICommand<Result<DeleteUserResult>>;

public record DeleteUserResult(bool IsSuccess);

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

internal class DeleteUserHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteUserCommand, Result<DeleteUserResult>>
{
    public async Task<Result<DeleteUserResult>> HandleAsync(
        DeleteUserCommand command,
        CancellationToken cancellationToken
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var usuario = await userRepository.FirstOrDefaultAsync(
            u => u.Id == command.UserId,
            asNoTracking: false,
            cancellationToken
        );

        if (usuario == null)
            return UserErrors.NotFound(command.UserId);

        // Soft delete - desactivar usuario
        usuario.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new DeleteUserResult(true);
    }
}
