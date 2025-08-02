using Module = Identity.Modules.Models.Module;

namespace Identity.Data.Seed;

public static class InitialData
{
    public static IEnumerable<PermissionType> TiposPermisos =>
        [
            PermissionType.Create("Leer", "VIEW", "READ", "Permite visualizar registros"),
            PermissionType.Create("Crear", "ADD", "WRITE", "Permite crear nuevos registros"),
            PermissionType.Create(
                "Actualizar",
                "EDIT",
                "WRITE",
                "Permite modificar registros existentes"
            ),
            PermissionType.Create("Eliminar", "REMOVE", "ADMIN", "Permite eliminar registros"),
            PermissionType.Create("Administrar", "ADMIN", "ADMIN", "Acceso completo al módulo"),
        ];

    public static IEnumerable<Module> Modules =>
        [
            Module.Create("Catalog", "Gestión de productos y categorías", true),
            Module.Create("Basket", "Gestión de carritos de compra", true),
            Module.Create("Ordering", "Gestión de órdenes y pedidos", true),
            Module.Create("Identity", "Gestión de usuarios y permisos", true),
        ];

    public static IEnumerable<Role> Roles =>
        [
            Role.Create("Administrador", "Acceso completo al sistema", true),
            Role.Create("Usuario", "Acceso básico al sistema", true),
            Role.Create("Operador", "Acceso para operaciones específicas", true),
        ];
}
