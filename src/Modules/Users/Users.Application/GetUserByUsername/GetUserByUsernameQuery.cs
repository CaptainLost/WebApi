using Core.Application.Abstractions.Messaging.Queries;

namespace Users.Application.GetUserByUsername;

public sealed record GetUserByUsernameQuery(string Username) : IQuery<UserResponse>;
