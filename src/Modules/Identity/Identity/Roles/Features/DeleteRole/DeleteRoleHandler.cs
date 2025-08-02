using Shared.Data.UnitOfWork;

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

internal class DeleteRoleHandler(
    IdentityDbContext dbContext,
    IUnitOfWork unitOfWork,
    ILogger<DeleteRoleHandler> logger
) : ICommandHandler<DeleteRoleCommand, Result<DeleteRoleResult>>
{
    public async Task<Result<DeleteRoleResult>> HandleAsync(
        DeleteRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        // 1. Verificar que el rol existe Y está activo
        var role = await dbContext.Roles.FirstOrDefaultAsync(
            r => r.Id == command.RoleId && r.Enabled,
            cancellationToken
        );

        if (role == null)
            return RoleErrors.NotFound(command.RoleId);

        // 2. Verificar si el rol tiene usuarios asignados
        var hasAssignedUsers = await dbContext.UserRoles.AnyAsync(
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

        // 3. Usar la estrategia de ejecución de EF para manejar reintentos
        try
        {
            await unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    var activePermissions = await dbContext
                        .Permissions.Where(p => p.IdRole == command.RoleId && p.Enabled)
                        .ToListAsync(cancellationToken);

                    foreach (var permission in activePermissions)
                        permission.DeactivateByRoleDeletion();

                    role.Deactivate();

                    logger.LogInformation(
                        "Rol {RoleId} desactivado con {Count} permisos",
                        command.RoleId,
                        activePermissions.Count
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
