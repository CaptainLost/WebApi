using Core.Application.Abstractions.Messaging.Commands;

namespace Users.Application.Register;

public sealed record RegisterCommand(string Username, string Email, string Password) : ICommand;
