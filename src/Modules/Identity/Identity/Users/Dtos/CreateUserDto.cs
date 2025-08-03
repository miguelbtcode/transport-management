namespace Identity.Users.Dtos;

public record CreateUserDto(
    string Name,
    string Email,
    string Password,
    bool Enabled,
    List<Guid> RoleIds
);
