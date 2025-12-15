using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.BanUser;

public sealed record BanUserCommand(
    Guid UserId,
    string Reason,
    Guid BannedBy,
    DateTime ExpiresAt) : ICommand;
