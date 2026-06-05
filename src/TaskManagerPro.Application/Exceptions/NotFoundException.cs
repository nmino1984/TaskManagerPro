using System;

namespace TaskManagerPro.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resource, int id)
        : base($"{resource} with ID {id} was not found.") { }

    public NotFoundException(string message)
        : base(message) { }
}
