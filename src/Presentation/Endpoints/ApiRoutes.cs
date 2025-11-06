namespace Presentation.Endpoints;

internal static class ApiRoutes
{
    internal static class Auth
    {
        private const string Base = "api/auth";

        internal const string Login = $"{Base}/login";
        internal const string Logout = $"{Base}/logout";
        internal const string Register = $"{Base}/register";
        internal const string Session = $"{Base}/session";
    }
}
