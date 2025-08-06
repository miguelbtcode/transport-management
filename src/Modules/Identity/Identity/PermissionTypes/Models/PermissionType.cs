namespace Identity.PermissionTypes.Models;

public class PermissionType : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string Category { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    // Soft delete
    public bool Enabled { get; set; }

    // Full audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime LastModified { get; set; }
    public string LastModifiedBy { get; set; } = default!;

    // Navigation properties
    private readonly List<Permission> _permissions = [];
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

    private PermissionType() { }

    public static PermissionType Create(
        string name,
        string code,
        string category,
        string description,
        bool enabled = true
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        return new PermissionType
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code.ToUpper(),
            Category = category.ToUpper(),
            Description = description ?? string.Empty,
            Enabled = enabled,
        };
    }

    public void Update(string name, string code, string category, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        Name = name;
        Code = code.ToUpper();
        Category = category.ToUpper();
        Description = description ?? string.Empty;
    }

    public void Activate() => Enabled = true;

    public void Deactivate() => Enabled = false;
}
