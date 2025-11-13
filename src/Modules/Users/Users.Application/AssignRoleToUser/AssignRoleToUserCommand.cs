using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(string UserId, string RoleName) : ICommand;
