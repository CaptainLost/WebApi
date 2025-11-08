using Application.Abstractions.Messaging.Queries;

namespace Application.Users.GetUserByUsername;

public sealed record GetUserByUsernameQuery(string Username) : IQuery<UserResponse>;
