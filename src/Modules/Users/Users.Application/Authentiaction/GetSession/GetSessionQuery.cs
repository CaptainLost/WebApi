using Core.Application.Abstractions.Messaging.Queries;

namespace Users.Application.GetSession;

public sealed record GetSessionQuery : IQuery<SessionResponse>;
