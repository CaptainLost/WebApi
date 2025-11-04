using System.Net;
using Domain.Messaging;

namespace Domain.Users;

public static class UserErrors
{
    public static Error LoginFailed() => new(
        Code: "User.LoginFailed",
        Description: "Login failed. Invalid username or password.",
        StatusCode: HttpStatusCode.Unauthorized);

    public static Error UserAlreadyExists() => new Error(
        Code: "User.AlreadyExists",
        Description: "User already exists.",
        StatusCode: HttpStatusCode.Conflict);
}