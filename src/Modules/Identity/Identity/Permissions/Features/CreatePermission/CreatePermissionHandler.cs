using Identity.Permissions.Dtos;
using Identity.PermissionTypes.Models;

namespace Identity.Permissions.Features.CreatePermission;

public record CreatePermissionCommand(CreatePermissionDto Permission)
    : ICommand<Result<CreatePermissionResult>>;

public record CreatePermissionResult(Guid Id);

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Permission.RoleId).NotEmpty().WithMessage("RoleId es requerido");
        RuleFor(x => x.Permission.ModuleId).NotEmpty().WithMessage("ModuleId es requerido");
        RuleFor(x => x.Permission.PermissionTypeId)
            .NotEmpty()
            .WithMessage("PermissionTypeId es requerido");
    }
}

internal class CreatePermissionHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreatePermissionCommand, Result<CreatePermissionResult>>
{
    public async Task<Result<CreatePermissionResult>> HandleAsync(
        CreatePermissionCommand command,
        CancellationToken cancellationToken
    )
    {
        var roleRepository = unitOfWork.Repository<Role>();
        var moduleRepository = unitOfWork.Repository<Module>();
        var permissionTypeRepository = unitOfWork.Repository<PermissionType>();
        var permissionRepository = unitOfWork.Repository<Permission>();

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

        // Verificar que el permiso no existe ya
        var existingPermission = await permissionRepository.FirstOrDefaultAsync(
            p =>
                p.IdRole == command.Permission.RoleId
                && p.IdModule == command.Permission.ModuleId
                && p.IdPermissionType == command.Permission.PermissionTypeId,
            asNoTracking: true,
            cancellationToken
        );

        if (existingPermission != null)
            return new Error(
                "Permission.AlreadyExists",
                "El permiso ya existe para este rol, módulo y tipo"
            );

        // Crear el permiso
        var permissionId = await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var permission = Permission.Create(
                    command.Permission.RoleId,
                    command.Permission.ModuleId,
                    command.Permission.PermissionTypeId,
                    command.Permission.Enabled
                );

                await permissionRepository.AddAsync(permission, cancellationToken);
                return permission.Id;
            },
            cancellationToken
        );

        return new CreatePermissionResult(permissionId);
    }
}
