using System.Net;
using Domain.Messaging;

namespace Domain.Errors;

public static class AuthenticationErrors
{
    public static Error LoginFailed() => new(
        Code: "Authentication.LoginFailed",
        Description: "Login failed. Invalid username or password.",
        StatusCode: HttpStatusCode.Unauthorized);

    public static Error UsernameAlreadyTaken() => new(
        Code: "Authentication.UsernameAlreadyTaken",
        Description: "Username is already taken.",
        StatusCode: HttpStatusCode.Conflict);

    public static Error EmailAlreadyTaken() => new(
        Code: "Authentication.EmailAlreadyTaken",
        Description: "Email is already taken.",
        StatusCode: HttpStatusCode.Conflict);

    public static Error RegistrationFailed(string details) => new(
        Code: "Authentication.RegistrationFailed",
        Description: $"Registration failed: {details}",
        StatusCode: HttpStatusCode.BadRequest);
}
