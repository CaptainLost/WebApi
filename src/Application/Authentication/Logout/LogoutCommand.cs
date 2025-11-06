using Application.Abstractions.Messaging.Commands;

namespace Application.Authentication.Logout;

public sealed record LogoutCommand : ICommand;
