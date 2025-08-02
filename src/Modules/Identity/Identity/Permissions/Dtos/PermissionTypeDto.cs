namespace Identity.Permissions.Dtos;

public record PermissionTypeDto(
    Guid Id,
    string Name,
    string Code,
    string Category,
    string Description
);
