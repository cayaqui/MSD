namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an operation is not allowed in the current state
/// </summary>
public class InvalidOperationException : ApplicationException
{
    public string? OperationName { get; }
    public string? CurrentState { get; }

    public InvalidOperationException()
        : base("The operation is not valid in the current state.")
    {
    }

    public InvalidOperationException(string message)
        : base(message)
    {
    }

    public InvalidOperationException(string operationName, string currentState)
        : base($"Operation '{operationName}' is not allowed in the current state: {currentState}")
    {
        OperationName = operationName;
        CurrentState = currentState;
    }
}
