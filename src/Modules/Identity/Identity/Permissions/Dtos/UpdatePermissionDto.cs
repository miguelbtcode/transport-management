namespace Identity.Permissions.Dtos;

public record UpdatePermissionDto(Guid RoleId, Guid ModuleId, Guid PermissionTypeId);
