using System.Net;
using Domain.Messaging;

namespace Domain.Errors;

public static class UserErrors
{
    public static Error NotFound(string username) => new(
        Code: "User.NotFound",
        Description: $"User with username '{username}' was not found.",
        StatusCode: HttpStatusCode.NotFound);
}
