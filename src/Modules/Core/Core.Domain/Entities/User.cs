using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public class User : IdentityUser
{
    public ICollection<Role> Roles { get; set; } = [];
}