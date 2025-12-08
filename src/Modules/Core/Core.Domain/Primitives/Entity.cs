namespace Core.Domain.Primitives;

/// <summary>
/// Represents the base class for all domain entities with unique identifier.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    private const int HashCodeMultiplier = 41;

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <remarks>
    /// Required for deserialization.
    /// </remarks>
    protected Entity()
    {
    }

    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public Guid Id { get; private init; }

    /// <summary>
    /// Checks if two entities are equal based on their identifier.
    /// </summary>
    /// <param name="first">The first entity.</param>
    /// <param name="second">The second entity.</param>
    /// <returns>True if both entities are equal, otherwise false.</returns>
    public static bool operator ==(Entity? first, Entity? second)
    {
        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (first is null || second is null)
        {
            return false;
        }

        return first.Equals(second);
    }

    /// <summary>
    /// Checks if two entities are not equal based on their identifier.
    /// </summary>
    /// <param name="first">The first entity.</param>
    /// <param name="second">The second entity.</param>
    /// <returns>True if entities are not equal, otherwise false.</returns>
    public static bool operator !=(Entity? first, Entity? second)
    {
        return !(first == second);
    }

    /// <inheritdoc />
    public bool Equals(Entity? other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.GetType() != GetType())
        {
            return false;
        }

        return other.Id == Id;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity entity)
        {
            return false;
        }

        return entity.Id == Id;
    }

    /// <summary>
    /// Returns the hash code for this entity.
    /// </summary>
    /// <returns>A hash code computed using a prime number multiplier to reduce collisions.</returns>
    public override int GetHashCode() => Id.GetHashCode() * HashCodeMultiplier;
}
