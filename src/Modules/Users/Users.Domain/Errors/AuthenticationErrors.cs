using Core.Domain.Messaging;

namespace Users.Domain.Errors;

public static class AuthenticationErrors
{
    public static Error LoginFailed() => new(
        Code: "Authentication.LoginFailed",
        Description: "Invalid credentials.");

    public static Error AccountLockedOut() => new(
        Code: "Authentication.AccountLockedOut",
        Description: "Account is locked due to multiple failed login attempts. Please try again later.");

    public static Error UsernameAlreadyTaken() => new(
        Code: "Authentication.UsernameAlreadyTaken",
        Description: "Username is already taken.");

    public static Error EmailAlreadyTaken() => new(
        Code: "Authentication.EmailAlreadyTaken",
        Description: "Email is already taken.");

    public static Error RegistrationFailed(string details) => new(
        Code: "Authentication.RegistrationFailed",
        Description: $"Registration failed: {details}");
}
