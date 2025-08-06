using Identity.Roles.Dtos;

namespace Identity.Users.Dtos;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    bool Enabled,
    List<RoleDto> Roles
);
