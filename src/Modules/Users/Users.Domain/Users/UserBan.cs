using Core.Domain.Primitives;

namespace Users.Domain.Users;

public sealed class UserBan : Entity
{
    public Guid UserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public Guid BannedBy { get; private set; }
    public DateTime BannedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UnbannedAt { get; private set; }
    public Guid? UnbannedBy { get; private set; }

    private UserBan()
    {
        
    }

    private UserBan(Guid id, Guid userId, string reason, Guid bannedBy, DateTime expiresAt)
        : base(id)
    {
        UserId = userId;
        Reason = reason;
        BannedBy = bannedBy;
        BannedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public static UserBan Create(Guid userId, string reason, Guid bannedBy, DateTime expiresAt)
    {
        return new UserBan(Guid.NewGuid(), userId, reason, bannedBy, expiresAt);
    }

    public void Deactivate(Guid unbannedBy)
    {
        UnbannedAt = DateTime.UtcNow;
        UnbannedBy = unbannedBy;
    }

    public bool IsCurrentlyActive()
    {
        return !UnbannedAt.HasValue && ExpiresAt > DateTime.UtcNow;
    }
}
