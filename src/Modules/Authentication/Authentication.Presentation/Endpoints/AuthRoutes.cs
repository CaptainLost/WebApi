using Core.Presentation.Endpoints;

namespace Authentication.Presentation.Endpoints;

public static class AuthRoutes
{
    public const string Base = $"{ApiRoutes.ApiPrefix}/auth";

    public const string Login = "login";
    public const string Logout = "logout";
    public const string Register = "register";
    public const string Session = "session";
}