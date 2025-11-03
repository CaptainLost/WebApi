using Domain.Messaging;

namespace Infrastructure.Messaging.InMemoryCommandDispatcher;

internal static class InMemoryCommandDispatcherErrors
{
    public static Error HandlerNotFound(Type commandType) => new(
            Code: "InMemoryCommandDispatcher.HandlerNotFound",
            Description: $"No handler registered for command type '{commandType.FullName}'.");
}