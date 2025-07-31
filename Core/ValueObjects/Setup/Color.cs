namespace Core.ValueObjects.Setup;

public record Color
{
    private readonly string _value;

    public string Value
    {
        get => _value;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Color cannot be null or empty", nameof(value));

            if (!IsValidHexColor(value))
                throw new ArgumentException("Color must be a valid hex color (e.g., #FF5733)", nameof(value));

            _value = value.ToUpperInvariant();
        }
    }

    public Color(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Color cannot be null or empty", nameof(value));
        _value = value.ToUpperInvariant();
    }

    public static bool IsValidHexColor(string value)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^#[0-9A-Fa-f]{6}$");
    }

    public static Color FromRgb(byte r, byte g, byte b)
    {
        return new Color($"#{r:X2}{g:X2}{b:X2}");
    }

    public (byte R, byte G, byte B) ToRgb()
    {
        var hex = Value.Substring(1); // Remove #
        var r = Convert.ToByte(hex.Substring(0, 2), 16);
        var g = Convert.ToByte(hex.Substring(2, 2), 16);
        var b = Convert.ToByte(hex.Substring(4, 2), 16);
        return (r, g, b);
    }

    // Predefined colors for disciplines
    public static Color Engineering => new("#0066CC");
    public static Color Procurement => new("#FF9900");
    public static Color Construction => new("#FF3333");
    public static Color Commissioning => new("#009900");
    public static Color ProjectManagement => new("#663399");
    public static Color QualityControl => new("#FF6666");
    public static Color Safety => new("#FFCC00");
    public static Color Environmental => new("#00CC66");

    public override string ToString() => Value;

    public static implicit operator string(Color color) => color.Value;
}