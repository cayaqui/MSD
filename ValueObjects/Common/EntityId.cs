namespace ValueObjects.Common;
public record EntityId
{
    public Guid Value { get; }

    public EntityId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("EntityId cannot be empty", nameof(value));

        Value = value;
    }

    public static EntityId Create() => new(Guid.NewGuid());

    public static EntityId From(Guid value) => new(value);

    public static implicit operator Guid(EntityId id) => id.Value;

    public override string ToString() => Value.ToString();
}
