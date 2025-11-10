namespace Core.Presentation.Endpoints;

public static class ApiRoutes
{
    private const string ApiPrefix = "api";

    public static class Auth
    {
        public const string Base = $"{ApiPrefix}/auth";
        public const string Login = "login";
        public const string Logout = "logout";
        public const string Register = "register";
        public const string Session = "session";
    }

    public static class Users
    {
        public const string Base = $"{ApiPrefix}/users";
        public const string GetByUsername = "{username}";
    }
}
