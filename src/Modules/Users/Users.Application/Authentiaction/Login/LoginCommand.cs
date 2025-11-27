using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Login;

public sealed record LoginCommand(string Username, string Password, bool IsPersistent = false) : ICommand;
