using Identity.PermissionTypes.Models;
using Identity.Roles.Dtos;

namespace Identity.Roles.Features.UpdateRole;

public record UpdateRoleCommand(Guid RoleId, UpdateRoleDto Role)
    : ICommand<Result<UpdateRoleResult>>;

public record UpdateRoleResult(bool IsSuccess);

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId es requerido");
        RuleFor(x => x.Role.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Role.Description).MaximumLength(255);
        RuleFor(x => x.Role.Permissions)
            .NotNull()
            .NotEmpty()
            .WithMessage("Debe especificar al menos un permiso");
        RuleForEach(x => x.Role.Permissions)
            .ChildRules(permission =>
            {
                permission.RuleFor(p => p.ModuleId).NotEmpty().WithMessage("ModuleId es requerido");
                permission
                    .RuleFor(p => p.PermissionTypeId)
                    .NotEmpty()
                    .WithMessage("PermissionTypeId es requerido");
            });
    }
}

internal class UpdateRoleHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateRoleCommand, Result<UpdateRoleResult>>
{
    public async Task<Result<UpdateRoleResult>> HandleAsync(
        UpdateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        // 1. Get repositories
        var roleRepository = unitOfWork.Repository<Role>();
        var moduleRepository = unitOfWork.Repository<Module>();
        var permissionTypeRepository = unitOfWork.Repository<PermissionType>();
        var permissionRepository = unitOfWork.Repository<Permission>();

        // 2. Verificar que el rol existe (tracking = true para modificar)
        var role = await roleRepository.FirstOrDefaultAsync(
            r => r.Id == command.RoleId && r.Enabled,
            asNoTracking: false,
            cancellationToken
        );

        if (role == null)
            return RoleErrors.NotFound(command.RoleId);

        // 3. Verificar si el nombre ya existe en otro rol (solo lectura)
        var existingRoleWithName = await roleRepository.FirstOrDefaultAsync(
            r => r.Name.ToLower() == command.Role.Name.ToLower() && r.Id != command.RoleId,
            asNoTracking: true,
            cancellationToken
        );

        if (existingRoleWithName != null)
            return RoleErrors.NameAlreadyExists(command.Role.Name);

        // 4. Verificar que todos los módulos existen y están activos (solo lectura)
        var moduleIds = command.Role.Permissions.Select(p => p.ModuleId).Distinct().ToList();
        var existingModuleIds = await moduleRepository
            .Query(m => moduleIds.Contains(m.Id) && m.Enabled, asNoTracking: true)
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        if (existingModuleIds.Count != moduleIds.Count)
        {
            return new Error(
                "Role.InvalidModules",
                "Uno o más módulos no existen o están inactivos"
            );
        }

        // 5. Verificar que todos los tipos de permiso existen (solo lectura)
        var permissionTypeIds = command
            .Role.Permissions.Select(p => p.PermissionTypeId)
            .Distinct()
            .ToList();
        var existingPermissionTypeIds = await permissionTypeRepository
            .Query(pt => permissionTypeIds.Contains(pt.Id), asNoTracking: true)
            .Select(pt => pt.Id)
            .ToListAsync(cancellationToken);

        if (existingPermissionTypeIds.Count != permissionTypeIds.Count)
        {
            return new Error(
                "Role.InvalidPermissionTypes",
                "Uno o más tipos de permiso no existen"
            );
        }

        // 6. Execute in transaction
        await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                // Actualizar información básica del rol (tracked entity)
                role.Update(command.Role.Name, command.Role.Description);

                // Eliminar todos los permisos existentes del rol (tracking = true para eliminar)
                var existingPermissions = await permissionRepository.GetAsync(
                    p => p.IdRole == command.RoleId,
                    asNoTracking: false,
                    cancellationToken
                );

                if (existingPermissions.Any())
                {
                    permissionRepository.RemoveRange(existingPermissions);
                }

                // Crear los nuevos permisos
                var newPermissions = command
                    .Role.Permissions.Select(p =>
                        Permission.Create(command.RoleId, p.ModuleId, p.PermissionTypeId)
                    )
                    .ToList();

                if (newPermissions.Any())
                {
                    await permissionRepository.AddRangeAsync(newPermissions, cancellationToken);
                }
            },
            cancellationToken
        );

        return new UpdateRoleResult(true);
    }
}
