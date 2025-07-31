namespace Core.ValueObjects.Common;

public record Percentage
{
    private readonly decimal _value;

    public decimal Value
    {
        get => _value;
        init
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Percentage must be between 0 and 100");
            _value = value;
        }
    }

    public Percentage(decimal value)
    {
        Value = value;
    }

    public static Percentage Zero => new(0m);
    public static Percentage OneHundred => new(100m);

    public static Percentage FromDecimal(decimal value)
    {
        return new Percentage(value * 100);
    }

    public decimal ToDecimal() => Value / 100;

    public Percentage Add(Percentage other)
    {
        var result = Value + other.Value;
        if (result > 100)
            throw new InvalidOperationException("Percentage cannot exceed 100%");
        return new Percentage(result);
    }

    public Percentage Subtract(Percentage other)
    {
        var result = Value - other.Value;
        if (result < 0)
            throw new InvalidOperationException("Percentage cannot be negative");
        return new Percentage(result);
    }

    public Money ApplyTo(Money money)
    {
        return money.Multiply(ToDecimal());
    }

    public override string ToString() => $"{Value:N2}%";

    public static implicit operator decimal(Percentage percentage) => percentage.Value;
    public static implicit operator Percentage(decimal value) => new(value);
}