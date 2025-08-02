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

internal class CreateRoleHandler(IdentityDbContext dbContext)
    : ICommandHandler<CreateRoleCommand, Result<CreateRoleResult>>
{
    public async Task<Result<CreateRoleResult>> HandleAsync(
        CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        // 1. Verify if the role already exists
        var existingRole = await dbContext.Roles.FirstOrDefaultAsync(
            r => r.Name.ToLower() == command.Role.Name.ToLower(),
            cancellationToken
        );

        if (existingRole != null)
            return RoleErrors.NameAlreadyExists(command.Role.Name);

        // 2. Verify each module exists and its enabled
        var moduleIds = command.Role.Permissions.Select(p => p.ModuleId).Distinct().ToList();
        var existingModules = await dbContext
            .Modules.Where(m => moduleIds.Contains(m.Id) && m.Enabled)
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        if (existingModules.Count != moduleIds.Count)
        {
            return new Error(
                "Role.InvalidModules",
                "Uno o más módulos no existen o están inactivos"
            );
        }

        // 3. Verify each permission type exists
        var permissionTypeIds = command
            .Role.Permissions.Select(p => p.PermissionTypeId)
            .Distinct()
            .ToList();
        var existingPermissionTypes = await dbContext
            .PermissionTypes.Where(pt => permissionTypeIds.Contains(pt.Id))
            .Select(pt => pt.Id)
            .ToListAsync(cancellationToken);

        if (existingPermissionTypes.Count != permissionTypeIds.Count)
        {
            return new Error(
                "Role.InvalidPermissionTypes",
                "Uno o más tipos de permiso no existen"
            );
        }

        // 4. Create the role
        var role = Role.Create(command.Role.Name, command.Role.Description, command.Role.Enabled);

        // 5. Add permissions to the role
        var permissions = command
            .Role.Permissions.Select(p =>
                Permission.Create(role.Id, p.ModuleId, p.PermissionTypeId)
            )
            .ToList();

        dbContext.Roles.Add(role);
        dbContext.Permissions.AddRange(permissions);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateRoleResult(role.Id);
    }
}
