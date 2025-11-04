using System.Net;
using Domain.Messaging;

namespace Infrastructure.Messaging;

public static class CommandDispatcherErrors
{
    public static Error HandlerNotFound(Type commandType) => new(
        "CommandDispatcher.HandlerNotFound",
        $"No handler was found for command type '{commandType.FullName}'.",
        HttpStatusCode.InternalServerError);
}
