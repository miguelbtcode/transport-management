namespace Identity.Permissions.Models;

public static class PermissionErrors
{
    public static Error NotFound(Guid permissionId) =>
        new("Permission.NotFound", $"El permiso con ID {permissionId} no fue encontrado");
}
