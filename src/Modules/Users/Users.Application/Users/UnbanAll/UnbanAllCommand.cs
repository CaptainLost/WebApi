using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.UnbanAll;

public sealed record UnbanAllCommand(Guid UserId, Guid UnbannedBy) : ICommand;
