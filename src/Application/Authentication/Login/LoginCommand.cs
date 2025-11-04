using Application.Abstractions.Messaging.Commands;

namespace Application.Authentication.Login;

public sealed record LoginCommand(string Username, string Password) : ICommand;
