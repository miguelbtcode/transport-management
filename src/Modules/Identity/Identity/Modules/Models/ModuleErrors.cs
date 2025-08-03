namespace Identity.Modules.Models;

public static class ModuleErrors
{
    public static Error NotFound(Guid moduleId) =>
        new("Module.NotFound", $"El módulo con ID {moduleId} no fue encontrado");

    public static Error NameAlreadyExists(string name) =>
        new("Module.NameAlreadyExists", $"Ya existe un módulo con el nombre '{name}'");
}
