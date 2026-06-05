using System;

namespace TaskManagerPro.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
