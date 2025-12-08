using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(Guid UserId, string RoleName) : ICommand;
