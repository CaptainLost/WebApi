using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.UnbanSingleUser;

public sealed record UnbanSingleUserCommand(Guid UserId, Guid BanId, Guid UnbannedBy) : ICommand;
