using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(string Username, string Email, string Password) : ICommand<string>;
