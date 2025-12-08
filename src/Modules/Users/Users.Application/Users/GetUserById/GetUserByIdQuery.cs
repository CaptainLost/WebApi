using Core.Application.Abstractions.Messaging.Queries;

namespace Users.Application.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid userId) : IQuery<GetUserByIdResponse>;
