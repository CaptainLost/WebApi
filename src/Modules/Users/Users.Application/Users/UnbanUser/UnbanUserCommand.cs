using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.UnbanUser;

public sealed record UnbanUserCommand(Guid UserId, Guid UnbannedBy) : ICommand;
