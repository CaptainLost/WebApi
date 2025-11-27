using Core.Domain.Primitives;

namespace Users.Domain.Entities;

public sealed class Role : Enumeration<Role>
{
    public static readonly Role User = new(1, "User");
    public static readonly Role Admin = new(2, "Admin");

    private Role(int id, string name)
        : base(id, name)
    {
    }

    private Role()
    {
    }

    public ICollection<Permission> Permissions { get; set; } = [];

    public ICollection<User> Users { get; set; } = [];
}