using Identity.Modules.Dtos;
using Identity.Permissions.Dtos;

namespace Identity.Roles.Dtos;

public record ModuleWithPermissionsDto(ModuleDto Module, List<PermissionTypeDto> Permissions);
