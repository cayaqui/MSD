namespace Core.ValueObjects.Setup;

public record WBSCode
{
    private readonly string _value;

    public string Value
    {
        get => _value;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("WBS code cannot be null or empty", nameof(value));

            if (!IsValidFormat(value))
                throw new ArgumentException("WBS code must follow the pattern: 1.2.3 format", nameof(value));

            _value = value;
        }
    }

    public WBSCode(string value)
    {
        // Inicializa el campo _value directamente para evitar CS8618
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("WBS code cannot be null or empty", nameof(value));

        if (!IsValidFormat(value))
            throw new ArgumentException("WBS code must follow the pattern: 1.2.3 format", nameof(value));

        _value = value;
    }

    private static bool IsValidFormat(string value)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^(\d+\.)*\d+$");
    }

    public int Level => Value.Split('.').Length;

    public string[] GetLevels() => Value.Split('.');

    public WBSCode? GetParent()
    {
        var levels = GetLevels();
        if (levels.Length <= 1)
            return null;

        var parentLevels = levels.Take(levels.Length - 1);
        return new WBSCode(string.Join(".", parentLevels));
    }

    public WBSCode CreateChild(int sequence)
    {
        if (sequence < 1)
            throw new ArgumentException("Sequence must be greater than 0", nameof(sequence));

        return new WBSCode($"{Value}.{sequence}");
    }

    public bool IsChildOf(WBSCode other)
    {
        return Value.StartsWith(other.Value + ".");
    }

    public bool IsParentOf(WBSCode other)
    {
        return other.IsChildOf(this);
    }

    public override string ToString() => Value;

    public static implicit operator string(WBSCode code) => code.Value;

    public static WBSCode CreateRoot(int sequence) => new(sequence.ToString());
}