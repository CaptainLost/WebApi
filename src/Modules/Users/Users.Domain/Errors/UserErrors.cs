using System.Net;
using Core.Domain.Messaging;

namespace Users.Domain.Errors;

public static class UserErrors
{
    public static Error NotFound(string username) => new(
        Code: "User.NotFound",
        Description: $"User with username '{username}' was not found.");
}
