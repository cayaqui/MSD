using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown for bad/invalid requests
/// </summary>
public class BadRequestException : ApplicationException
{
    public string? Code { get; }
    public object? Details { get; }

    public BadRequestException()
        : base("Bad request.")
    {
    }

    public BadRequestException(string message)
        : base(message)
    {
    }

    public BadRequestException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    public BadRequestException(string message, object details)
        : base(message)
    {
        Details = details;
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
