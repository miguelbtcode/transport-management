namespace Identity.Roles.Features.ReactivateRole;

public record ReactivateRoleCommand(Guid RoleId) : ICommand<Result<ReactivateRoleResult>>;

public record ReactivateRoleResult(bool IsSuccess, string Message);

public class ReactivateRoleCommandValidator : AbstractValidator<ReactivateRoleCommand>
{
    public ReactivateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId es requerido");
    }
}

internal class ReactivateRoleHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<ReactivateRoleCommand, Result<ReactivateRoleResult>>
{
    public async Task<Result<ReactivateRoleResult>> HandleAsync(
        ReactivateRoleCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var roleRepository = unitOfWork.Repository<Role>();

        var role = await roleRepository.FirstOrDefaultAsync(
            u => u.Id == command.RoleId,
            asNoTracking: false,
            cancellationToken
        );

        if (role == null)
            return RoleErrors.NotFound(command.RoleId);

        if (role.Enabled)
            return RoleErrors.AlreadyActive;

        role.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new ReactivateRoleResult(true, "Usuario reactivado exitosamente");
    }
}
