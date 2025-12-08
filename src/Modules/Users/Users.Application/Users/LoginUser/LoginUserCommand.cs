using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.LoginUser;

public sealed record LoginUserCommand(string Username, string Password) : ICommand<string>;
