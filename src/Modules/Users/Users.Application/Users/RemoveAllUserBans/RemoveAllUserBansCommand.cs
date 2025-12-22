using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.UnbanAll;

public sealed record RemoveAllUserBansCommand(Guid UserId, Guid BanRemoverId) : ICommand;
