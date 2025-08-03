namespace Identity.Roles.Features.DeleteRole;

public record DeleteRoleCommand(Guid RoleId) : ICommand<Result<DeleteRoleResult>>;

public record DeleteRoleResult(bool IsSuccess);

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId es requerido");
    }
}

internal class DeleteRoleHandler(IUnitOfWork unitOfWork, ILogger<DeleteRoleHandler> logger)
    : ICommandHandler<DeleteRoleCommand, Result<DeleteRoleResult>>
{
    public async Task<Result<DeleteRoleResult>> HandleAsync(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        // 1. Get repositories
        var roleRepository = unitOfWork.Repository<Role>();
        var userRoleRepository = unitOfWork.Repository<UserRole>();
        var permissionRepository = unitOfWork.Repository<Permission>();

        // 2. Verificar que el rol existe y está activo
        var role = await roleRepository.FirstOrDefaultAsync(
            r => r.Id == command.RoleId && r.Enabled,
            asNoTracking: false,
            cancellationToken
        );

        if (role == null)
            return RoleErrors.NotFound(command.RoleId);

        // 3. Verificar si el rol tiene usuarios asignados
        var hasAssignedUsers = await userRoleRepository.AnyAsync(
            ur => ur.IdRole == command.RoleId,
            cancellationToken
        );

        if (hasAssignedUsers)
        {
            return new Error(
                "Role.HasAssignedUsers",
                "No se puede eliminar un rol que tiene usuarios asignados"
            );
        }

        try
        {
            // 4. Ejecutar operación en transacción
            await unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    var activePermissions = await permissionRepository.GetAsync(
                        p => p.IdRole == command.RoleId && p.Enabled,
                        asNoTracking: false,
                        cancellationToken
                    );

                    foreach (var permission in activePermissions)
                        permission.DeactivateByRoleDeletion();

                    role.Deactivate();

                    logger.LogInformation(
                        "Rol {RoleId} desactivado con {Count} permisos",
                        command.RoleId,
                        activePermissions.Count()
                    );
                },
                cancellationToken
            );

            return new DeleteRoleResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error eliminando rol {RoleId}", command.RoleId);
            throw;
        }
    }
}
