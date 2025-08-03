namespace Identity.Modules.Dtos;

public record CreateModuleDto(string Name, string Description, bool Enabled = true);
