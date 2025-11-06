using Application.Abstractions.Messaging.Queries;

namespace Application.Authentication.GetSession;

public sealed record GetSessionQuery : IQuery<SessionResponse>;

public sealed record SessionResponse(
    bool IsAuthenticated,
    string? Username,
    string? UserId);
