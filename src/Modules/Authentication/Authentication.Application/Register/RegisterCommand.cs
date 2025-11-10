using Core.Application.Abstractions.Messaging.Commands;

namespace Authentication.Application.Register;

public sealed record RegisterCommand(string Username, string Email, string Password) : ICommand;
