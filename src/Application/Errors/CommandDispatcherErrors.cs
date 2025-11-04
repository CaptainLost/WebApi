using System.Net;
using Domain.Messaging;

namespace Application.Errors;

public static class CommandDispatcherErrors
{
    public static Error HandlerNotFound(Type commandType) => new(
        "CommandDispatcher.HandlerNotFound",
        $"No handler was found for command type '{commandType.FullName}'.",
        HttpStatusCode.InternalServerError);
}
