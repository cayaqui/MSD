using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Common;


/// <summary>
/// Generic result type for service operations
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; protected set; }

    /// <summary>
    /// Indicates if the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? Error { get; protected set; }

    /// <summary>
    /// List of errors
    /// </summary>
    public List<string> Errors { get; protected set; } = new();

    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        if (!string.IsNullOrEmpty(error))
        {
            Errors.Add(error);
        }
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    public static Result Failure(List<string> errors)
    {
        var result = new Result(false);
        result.Errors = errors;
        result.Error = string.Join("; ", errors);
        return result;
    }
}

/// <summary>
/// Generic result type with value for service operations
/// </summary>
public class Result<T> : Result
{
    /// <summary>
    /// The value of the result
    /// </summary>
    public T? Value { get; private set; }

    protected Result(bool isSuccess, T? value = default, string? error = null)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with value
    /// </summary>
    public static Result<T> Success(T value) => new(true, value);

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public new static Result<T> Failure(string error) => new(false, default, error);

    /// <summary>
    /// Creates a failed result with multiple errors
    /// </summary>
    public new static Result<T> Failure(List<string> errors)
    {
        var result = new Result<T>(false, default);
        result.Errors = errors;
        result.Error = string.Join("; ", errors);
        return result;
    }

    /// <summary>
    /// Implicit conversion to T
    /// </summary>
    public static implicit operator T?(Result<T> result) => result.Value;
}
