using Identity.Modules.Models;

namespace Identity.Modules.Features.DeleteModule;

public record DeleteModuleCommand(Guid ModuleId) : ICommand<Result<DeleteModuleResult>>;

public record DeleteModuleResult(bool IsSuccess);

public class DeleteModuleCommandValidator : AbstractValidator<DeleteModuleCommand>
{
    public DeleteModuleCommandValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty().WithMessage("ModuleId es requerido");
    }
}

internal class DeleteModuleHandler(IUnitOfWork unitOfWork, ILogger<DeleteModuleHandler> logger)
    : ICommandHandler<DeleteModuleCommand, Result<DeleteModuleResult>>
{
    public async Task<Result<DeleteModuleResult>> HandleAsync(
        DeleteModuleCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var moduleRepository = unitOfWork.Repository<Module>();
            var permissionRepository = unitOfWork.Repository<Permission>();

            // Verificar que el módulo existe y está activo
            var module = await moduleRepository.FirstOrDefaultAsync(
                m => m.Id == command.ModuleId && m.Enabled,
                asNoTracking: false,
                cancellationToken
            );

            if (module == null)
                return ModuleErrors.NotFound(command.ModuleId);

            // Verificar si el módulo tiene permisos asignados
            var hasAssignedPermissions = await permissionRepository.AnyAsync(
                p => p.IdModule == command.ModuleId && p.Enabled,
                cancellationToken
            );

            if (hasAssignedPermissions)
            {
                return new Error(
                    "Module.HasAssignedPermissions",
                    "No se puede eliminar un módulo que tiene permisos asignados"
                );
            }

            // Ejecutar en transacción
            await unitOfWork.ExecuteInTransactionAsync(
                () =>
                {
                    module.Deactivate();

                    logger.LogInformation(
                        "Módulo {ModuleId} desactivado exitosamente",
                        command.ModuleId
                    );
                    return Task.CompletedTask;
                },
                cancellationToken
            );

            return new DeleteModuleResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error eliminando módulo {ModuleId}", command.ModuleId);
            throw;
        }
    }
}
