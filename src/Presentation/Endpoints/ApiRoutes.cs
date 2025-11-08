namespace Presentation.Endpoints;

internal static class ApiRoutes
{
    private const string ApiPrefix = "api";

    internal static class Auth
    {
        internal const string Base = $"{ApiPrefix}/auth";
        internal const string Login = "login";
        internal const string Logout = "logout";
        internal const string Register = "register";
        internal const string Session = "session";
    }

    internal static class Users
    {
        internal const string Base = $"{ApiPrefix}/users";
        internal const string GetByUsername = "{username}";
    }
}
