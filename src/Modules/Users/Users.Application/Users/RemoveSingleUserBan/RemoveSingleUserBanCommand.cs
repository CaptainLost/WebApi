using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.UnbanSingleUser;

public sealed record RemoveSingleUserBanCommand(Guid UserId, Guid BanId, Guid BanRemoverId) : ICommand;
