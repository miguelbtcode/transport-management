using Identity.Permissions.Dtos;
using Identity.PermissionTypes.Models;

namespace Identity.Permissions.Features.UpdatePermission;

public record UpdatePermissionCommand(Guid PermissionId, UpdatePermissionDto Permission)
    : ICommand<Result<UpdatePermissionResult>>;

public record UpdatePermissionResult(bool IsSuccess);

public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.PermissionId).NotEmpty().WithMessage("PermissionId es requerido");
        RuleFor(x => x.Permission.RoleId).NotEmpty().WithMessage("RoleId es requerido");
        RuleFor(x => x.Permission.ModuleId).NotEmpty().WithMessage("ModuleId es requerido");
        RuleFor(x => x.Permission.PermissionTypeId)
            .NotEmpty()
            .WithMessage("PermissionTypeId es requerido");
    }
}

internal class UpdatePermissionHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdatePermissionCommand, Result<UpdatePermissionResult>>
{
    public async Task<Result<UpdatePermissionResult>> HandleAsync(
        UpdatePermissionCommand command,
        CancellationToken cancellationToken
    )
    {
        var roleRepository = unitOfWork.Repository<Role>();
        var moduleRepository = unitOfWork.Repository<Module>();
        var permissionTypeRepository = unitOfWork.Repository<PermissionType>();
        var permissionRepository = unitOfWork.Repository<Permission>();

        // Verificar que el permiso existe
        var permission = await permissionRepository.FirstOrDefaultAsync(
            p => p.Id == command.PermissionId && p.Enabled,
            asNoTracking: false,
            cancellationToken
        );

        if (permission == null)
            return PermissionErrors.NotFound(command.PermissionId);

        // Verificar que el rol existe y está activo
        var roleExists = await roleRepository.AnyAsync(
            r => r.Id == command.Permission.RoleId && r.Enabled,
            cancellationToken
        );

        if (!roleExists)
            return new Error(
                "Permission.InvalidRole",
                "El rol especificado no existe o está inactivo"
            );

        // Verificar que el módulo existe y está activo
        var moduleExists = await moduleRepository.AnyAsync(
            m => m.Id == command.Permission.ModuleId && m.Enabled,
            cancellationToken
        );

        if (!moduleExists)
            return new Error(
                "Permission.InvalidModule",
                "El módulo especificado no existe o está inactivo"
            );

        // Verificar que el tipo de permiso existe
        var permissionTypeExists = await permissionTypeRepository.AnyAsync(
            pt => pt.Id == command.Permission.PermissionTypeId,
            cancellationToken
        );

        if (!permissionTypeExists)
            return new Error(
                "Permission.InvalidPermissionType",
                "El tipo de permiso especificado no existe"
            );

        // Verificar que no existe otro permiso con la misma combinación
        var duplicatePermission = await permissionRepository.FirstOrDefaultAsync(
            p =>
                p.IdRole == command.Permission.RoleId
                && p.IdModule == command.Permission.ModuleId
                && p.IdPermissionType == command.Permission.PermissionTypeId
                && p.Id != command.PermissionId,
            asNoTracking: true,
            cancellationToken
        );

        if (duplicatePermission != null)
            return new Error(
                "Permission.AlreadyExists",
                "Ya existe otro permiso con esta combinación"
            );

        // Actualizar el permiso
        await unitOfWork.ExecuteInTransactionAsync(
            () =>
            {
                permission.Update(
                    command.Permission.RoleId,
                    command.Permission.ModuleId,
                    command.Permission.PermissionTypeId
                );
                return Task.CompletedTask;
            },
            cancellationToken
        );

        return new UpdatePermissionResult(true);
    }
}
