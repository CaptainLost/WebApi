using Core.Application.Abstractions.Messaging.Queries;

namespace Authentication.Application.GetSession;

public sealed record GetSessionQuery : IQuery<SessionResponse>;
