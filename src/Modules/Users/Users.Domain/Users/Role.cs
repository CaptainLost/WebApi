using Core.Domain.Primitives;

namespace Users.Domain.Users;

public sealed class Role : Enumeration<Role>
{
    public static readonly Role Registered = new(1, "Registered");
    public static readonly Role Administrator = new(2, "Administrator");
    
    public static Role DefaultUserRole => Registered;
    
    public ICollection<Permission> Permissions { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];

    private Role(int id, string name)
        : base(id, name)
    {
    }

    private Role()
    {
    }
}
