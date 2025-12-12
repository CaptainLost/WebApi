using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Users.CreateUser;

public sealed record CreateUserCommand(string Username, string Email, string Password) : ICommand<string>;
