namespace Identity.Roles.Models;

public static class RoleErrors
{
    public static Error NotFound(Guid id) =>
        new("Role.NotFound", $"Rol con ID '{id}' no fue encontrado");

    public static Error InvalidRoles =>
        new("Role.InvalidRoles", "Uno o más roles no existen o están inactivos");

    public static Error NameAlreadyExists(string name) =>
        new("Role.NameAlreadyExists", $"Ya existe un rol con el nombre '{name}'");

    public static Error AlreadyActive =>
        new("Role.AlreadyActive", "El usuario ya se encuentra activo");

    public static Error AlreadyInactive =>
        new("Role.AlreadyInactive", "El usuario ya se encuentra inactivo");
}
