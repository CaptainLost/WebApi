using Application.Abstractions.Messaging.Commands;

namespace Application.Users.Login;

public sealed record LoginCommand(string Username, string Password) : ICommand;