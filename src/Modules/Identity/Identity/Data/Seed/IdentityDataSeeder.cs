namespace Identity.Data.Seed;

public class IdentityDataSeeder(IdentityDbContext dbContext) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedPermissionTypesAsync();
        await SeedModulesAsync();
        await SeedRolesAsync();
        await SeedDefaultUserAsync();
        await SeedDefaultPermissionsAsync();
    }

    private async Task SeedPermissionTypesAsync()
    {
        if (!await dbContext.PermissionTypes.AnyAsync())
        {
            await dbContext.PermissionTypes.AddRangeAsync(InitialData.TiposPermisos);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedModulesAsync()
    {
        if (!await dbContext.Modules.AnyAsync())
        {
            await dbContext.Modules.AddRangeAsync(InitialData.Modules);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        if (!await dbContext.Roles.AnyAsync())
        {
            await dbContext.Roles.AddRangeAsync(InitialData.Roles);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedDefaultUserAsync()
    {
        if (!await dbContext.Users.AnyAsync())
        {
            // Crear usuario administrador por defecto
            var adminUser = User.Create(
                "Administrador",
                "admin@eshop.com",
                BCrypt.Net.BCrypt.HashPassword("Admin123!")
            );

            // Asignar rol de administrador
            var adminRole = await dbContext.Roles.FirstAsync(r => r.Name == "Administrador");
            adminUser.AssignRole(adminRole.Id);

            dbContext.Users.Add(adminUser);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedDefaultPermissionsAsync()
    {
        if (!await dbContext.Permissions.AnyAsync())
        {
            var roles = await dbContext.Roles.Where(r => r.Enabled).ToListAsync();
            var modules = await dbContext.Modules.Where(m => m.Enabled).ToListAsync();
            var permissionTypes = await dbContext.PermissionTypes.ToListAsync();

            var permissions = new List<Permission>();

            foreach (var role in roles)
            {
                switch (role.Name)
                {
                    case "Administrador":
                        // Administrador: Acceso completo a todos los módulos
                        permissions.AddRange(
                            modules.Select(module =>
                                Permission.Create(
                                    role.Id,
                                    module.Id,
                                    permissionTypes.First(pt => pt.Name == "Administrar").Id
                                )
                            )
                        );
                        break;

                    case "Usuario":
                        // Usuario: Solo lectura en Catalog y Basket, crear/leer en Ordering
                        foreach (var module in modules)
                        {
                            switch (module.Name)
                            {
                                case "Catalog":
                                case "Basket":
                                    // Solo lectura
                                    permissions.Add(
                                        Permission.Create(
                                            role.Id,
                                            module.Id,
                                            permissionTypes.First(pt => pt.Name == "Leer").Id
                                        )
                                    );
                                    break;

                                case "Ordering":
                                    // Crear y leer pedidos
                                    permissions.Add(
                                        Permission.Create(
                                            role.Id,
                                            module.Id,
                                            permissionTypes.First(pt => pt.Name == "Leer").Id
                                        )
                                    );
                                    permissions.Add(
                                        Permission.Create(
                                            role.Id,
                                            module.Id,
                                            permissionTypes.First(pt => pt.Name == "Crear").Id
                                        )
                                    );
                                    break;
                            }
                        }
                        break;

                    case "Operador":
                        // Operador: CRUD en Catalog, Basket y Ordering (sin Identity)
                        foreach (var module in modules.Where(m => m.Name != "Identity"))
                        {
                            // Permisos CRUD básicos
                            permissions.Add(
                                Permission.Create(
                                    role.Id,
                                    module.Id,
                                    permissionTypes.First(pt => pt.Name == "Leer").Id
                                )
                            );
                            permissions.Add(
                                Permission.Create(
                                    role.Id,
                                    module.Id,
                                    permissionTypes.First(pt => pt.Name == "Crear").Id
                                )
                            );
                            permissions.Add(
                                Permission.Create(
                                    role.Id,
                                    module.Id,
                                    permissionTypes.First(pt => pt.Name == "Actualizar").Id
                                )
                            );
                            permissions.Add(
                                Permission.Create(
                                    role.Id,
                                    module.Id,
                                    permissionTypes.First(pt => pt.Name == "Eliminar").Id
                                )
                            );
                        }
                        break;
                }
            }

            await dbContext.Permissions.AddRangeAsync(permissions);
            await dbContext.SaveChangesAsync();
        }
    }
}
