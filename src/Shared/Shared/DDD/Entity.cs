using Shared.Contracts.DDD;

namespace Shared.DDD;

public abstract class Entity<T> : IEntity<T>
{
    public required T Id { get; set; }
    public bool Enabled { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedReason { get; set; }
}
