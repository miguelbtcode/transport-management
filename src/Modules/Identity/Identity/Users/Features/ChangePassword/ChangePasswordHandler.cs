using Identity.Authentication.Services;

namespace Identity.Users.Features.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword)
    : ICommand<Result<ChangePasswordResult>>;

public record ChangePasswordResult(bool IsSuccess);

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
    }
}

internal class ChangePasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    : ICommandHandler<ChangePasswordCommand, Result<ChangePasswordResult>>
{
    public async Task<Result<ChangePasswordResult>> HandleAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var userRepository = unitOfWork.Repository<User>();

        var user = await userRepository.FirstOrDefaultAsync(
            u => u.Id == command.UserId,
            asNoTracking: false, // Necesitamos tracking para modificar
            cancellationToken
        );

        if (user == null)
            return UserErrors.NotFound(command.UserId);

        // Verificar contraseña actual
        if (!passwordHasher.VerifyPassword(command.CurrentPassword, user.Password))
            return UserErrors.InvalidCredentials;

        // Cambiar contraseña
        var newHashedPassword = passwordHasher.HashPassword(command.NewPassword);
        user.ChangePassword(newHashedPassword);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new ChangePasswordResult(true);
    }
}
