namespace Identity.Users.Models;

public class User : Aggregate<Guid>
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Password { get; private set; } = default!;

    // Soft delete
    public bool Enabled { get; set; }

    // Full audit fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime LastModified { get; set; }
    public string LastModifiedBy { get; set; } = default!;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedReason { get; set; }

    // Navigation properties
    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() { }

    public static User Create(string name, string email, string hashedPassword, bool enabled = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email.ToLower(),
            Password = hashedPassword,
            Enabled = enabled,
        };

        user.AddDomainEvent(new UserCreatedEvent(user));
        return user;
    }

    public void UpdateUserInformation(string name, string email, List<Guid> roleIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(roleIds);

        Name = name;
        Email = email.ToLower();

        AddDomainEvent(new UserUpdatedEvent(this, roleIds));
    }

    public void ChangePassword(string newHashedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newHashedPassword);
        Password = newHashedPassword;
    }

    public void Activate() => Enabled = true;

    public void Deactivate(string reason = "Account deactivated")
    {
        Enabled = false;
        DeletedAt = DateTime.UtcNow;
        DeletedReason = reason;
    }

    public void SuspendAccount(string reason = "Account suspended")
    {
        Enabled = false;
        DeletedAt = DateTime.UtcNow;
        DeletedReason = reason;
    }

    public void AssignRole(Guid roleId)
    {
        if (_userRoles.Any(ur => ur.IdRole == roleId))
            return;

        var userRole = UserRole.Create(Id, roleId);
        _userRoles.Add(userRole);
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.IdRole == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
        }
    }
}
