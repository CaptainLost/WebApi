using Core.Application.Abstractions.Messaging.Commands;

namespace Authentication.Application.Login;

public sealed record LoginCommand(string Username, string Password, bool IsPersistent = false) : ICommand;
