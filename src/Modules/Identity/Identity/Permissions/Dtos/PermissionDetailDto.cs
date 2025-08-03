namespace Identity.Permissions.Dtos;

public record PermissionDetailDto(
    Guid Id,
    Guid RoleId,
    string RoleName,
    Guid ModuleId,
    string ModuleName,
    Guid PermissionTypeId,
    string PermissionTypeName,
    string PermissionTypeCode,
    DateTime DateAssigned,
    bool Enabled
);
