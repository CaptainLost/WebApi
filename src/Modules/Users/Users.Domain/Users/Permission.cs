using Core.Domain.Primitives;

namespace Users.Domain.Users;

public sealed class Permission : Enumeration<Permission>
{
    public static readonly Permission GetUser = new(1, "users:read");
    public static readonly Permission ModifyUser = new(2, "users:update");
    public static readonly Permission CreateUser = new(3, "users:create");
    public static readonly Permission DeleteUser = new(4, "users:delete");
    public static readonly Permission AssignRole = new(5, "users:assign-role");
    public static readonly Permission GetUserList = new(6, "users:list");
    public static readonly Permission BanUser = new(7, "users:ban");
    public static readonly Permission UnbanUser = new(8, "users:unban");
    public static readonly Permission RemoveSingleBan = new(9, "users:remove-single-ban");
    public static readonly Permission RemoveAllUserBans = new(10, "users:remove-all-bans");

    private Permission(int id, string name)
        : base(id, name)
    {
    }

    private Permission()
    {
    }
}
