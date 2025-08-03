using Identity.PermissionTypes.Models;
using Identity.Roles.Dtos;

namespace Identity.Roles.Features.CreateRole;

public record CreateRoleCommand(CreateRoleDto Role) : ICommand<Result<CreateRoleResult>>;

public record CreateRoleResult(Guid Id);

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
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

internal class CreateRoleHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoleCommand, Result<CreateRoleResult>>
{
    public async Task<Result<CreateRoleResult>> HandleAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        // 1. Get repositories
        var roleRepository = unitOfWork.Repository<Role>();
        var moduleRepository = unitOfWork.Repository<Module>();
        var permissionTypeRepository = unitOfWork.Repository<PermissionType>();
        var permissionRepository = unitOfWork.Repository<Permission>();

        // 2. Verify if the role already exists (solo lectura)
        var existingRole = await roleRepository.FirstOrDefaultAsync(
            r => r.Name.ToLower() == command.Role.Name.ToLower(),
            asNoTracking: true,
            cancellationToken
        );

        if (existingRole != null)
            return RoleErrors.NameAlreadyExists(command.Role.Name);

        // 3. Verify each module exists and is enabled (solo lectura)
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

        // 4. Verify each permission type exists (solo lectura)
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

        // 5. Execute in transaction
        var roleId = await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                // Create the role
                var role = Role.Create(
                    command.Role.Name,
                    command.Role.Description,
                    command.Role.Enabled
                );
                await roleRepository.AddAsync(role, cancellationToken);

                // Add permissions to the role
                var permissions = command
                    .Role.Permissions.Select(p =>
                        Permission.Create(role.Id, p.ModuleId, p.PermissionTypeId)
                    )
                    .ToList();

                await permissionRepository.AddRangeAsync(permissions, cancellationToken);

                return role.Id;
            },
            cancellationToken
        );

        return new CreateRoleResult(roleId);
    }
}
