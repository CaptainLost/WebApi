using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities;

public sealed class User : IdentityUser
{
    public ICollection<Role> Roles { get; set; } = [];
}