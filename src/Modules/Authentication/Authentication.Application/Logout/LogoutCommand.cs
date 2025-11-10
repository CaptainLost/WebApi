using Core.Application.Abstractions.Messaging.Commands;

namespace Authentication.Application.Logout;

public sealed record LogoutCommand : ICommand;
