using Core.Presentation.Endpoints;

namespace Users.Presentation.Endpoints;

public static class UsersRoutes
{
    public const string Base = $"{ApiRoutes.ApiPrefix}/users";

    public const string GetByUsername = "{username}";
    public const string GetUserList = "";
    public const string AssignRole = "{userId}/roles";
    public const string Ban = "{userId}/ban";
    public const string UnbanAll = "{userId}/unban-all";
    public const string UnbanSingle = "{userId}/bans/{banId}";

    public const string Login = "login";
    public const string Register = "register";
}
