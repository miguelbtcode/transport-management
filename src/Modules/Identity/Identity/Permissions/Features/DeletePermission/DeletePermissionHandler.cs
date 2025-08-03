namespace Identity.Permissions.Features.DeletePermission;

public record DeletePermissionCommand(Guid PermissionId) : ICommand<Result<DeletePermissionResult>>;

public record DeletePermissionResult(bool IsSuccess);

public class DeletePermissionCommandValidator : AbstractValidator<DeletePermissionCommand>
{
    public DeletePermissionCommandValidator()
    {
        RuleFor(x => x.PermissionId).NotEmpty().WithMessage("PermissionId es requerido");
    }
}

internal class DeletePermissionHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeletePermissionHandler> logger
) : ICommandHandler<DeletePermissionCommand, Result<DeletePermissionResult>>
{
    public async Task<Result<DeletePermissionResult>> HandleAsync(
        DeletePermissionCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var permissionRepository = unitOfWork.Repository<Permission>();

            var permission = await permissionRepository.FirstOrDefaultAsync(
                p => p.Id == command.PermissionId && p.Enabled,
                asNoTracking: false,
                cancellationToken
            );

            if (permission == null)
                return PermissionErrors.NotFound(command.PermissionId);

            await unitOfWork.ExecuteInTransactionAsync(
                () =>
                {
                    permission.Deactivate();

                    logger.LogInformation(
                        "Permiso {PermissionId} desactivado exitosamente",
                        command.PermissionId
                    );
                    return Task.CompletedTask;
                },
                cancellationToken
            );

            return new DeletePermissionResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error eliminando permiso {PermissionId}", command.PermissionId);
            throw;
        }
    }
}
