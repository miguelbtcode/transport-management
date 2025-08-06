using Shared.Contracts.DDD;

namespace Shared.DDD;

public abstract class Entity<T> : IEntity<T>
{
    public required T Id { get; set; }
}
