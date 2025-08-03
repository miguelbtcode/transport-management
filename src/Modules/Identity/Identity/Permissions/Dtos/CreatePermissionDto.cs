namespace Identity.Permissions.Dtos;

public record CreatePermissionDto(
    Guid RoleId,
    Guid ModuleId,
    Guid PermissionTypeId,
    bool Enabled = true
);
