namespace Identity.Roles.Dtos;

public record CreateRoleDto(
    string Name,
    string Description,
    bool Enabled,
    List<RolePermissionDto> Permissions
);

public record RolePermissionDto(Guid ModuleId, Guid PermissionTypeId);
