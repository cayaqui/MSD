namespace Core.Results;

/// <summary>
/// Representa el resultado de una operación
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string Error { get; } = string.Empty;
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}

/// <summary>
/// Representa el resultado de una operación con un valor de retorno
/// </summary>
public class Result<T> : Result
{
    internal Result(T value, bool isSuccess, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; }
}