namespace Users.Domain.Users;

public sealed class RoleUser
{
    public int RoleId { get; set; }

    public Guid UserId { get; set; }
}
