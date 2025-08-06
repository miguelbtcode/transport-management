namespace Identity.Modules.Models;

public class Module : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    // Soft delete
    public bool Enabled { get; set; }

    // Navigation properties
    private readonly List<Permission> _permissions = [];
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

    public static Module Create(string name, string description, bool enabled = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Module
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description ?? string.Empty,
            Enabled = enabled,
        };
    }

    public void Update(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Description = description ?? string.Empty;
    }

    public void Activate() => Enabled = true;

    public void Deactivate() => Enabled = false;
}
