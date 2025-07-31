namespace ValueObjects.Setup;

public record ProjectCode
{
    private readonly string _value;

    public string Value
    {
        get => _value;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Project code cannot be null or empty", nameof(value));

            if (value.Length < 3 || value.Length > 20)
                throw new ArgumentException("Project code must be between 3 and 20 characters", nameof(value));

            if (!IsValidFormat(value))
                throw new ArgumentException("Project code must contain only letters, numbers, and hyphens", nameof(value));

            _value = value.ToUpperInvariant();
        }
    }

    public ProjectCode(string value)
    {
        _value = null!; // Inicialización para evitar advertencia CS8618
        Value = value;
    }

    private static bool IsValidFormat(string value)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9\-]+$");
    }

    public static ProjectCode Create(string prefix, int year, int sequence)
    {
        var code = $"{prefix}-{year:0000}-{sequence:0000}";
        return new ProjectCode(code);
    }

    public override string ToString() => Value;

    public static implicit operator string(ProjectCode code) => code.Value;
}