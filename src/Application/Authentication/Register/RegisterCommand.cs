using Application.Abstractions.Messaging.Commands;

namespace Application.Authentication.Register;

public sealed record RegisterCommand(string Username, string Email, string Password) : ICommand;
