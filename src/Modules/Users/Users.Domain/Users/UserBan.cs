using Core.Domain.Messaging;
using Core.Domain.Primitives;

namespace Users.Domain.Users;

public sealed class UserBan : Entity
{
    public Guid UserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public Guid BanImposerId { get; private set; }
    public DateTime BannedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UnbannedAt { get; private set; }
    public Guid? BanRemoverId { get; private set; }

    private UserBan()
    {

    }

    private UserBan(Guid id, Guid userId, string reason, Guid bannedBy, DateTime expiresAt)
        : base(id)
    {
        UserId = userId;
        Reason = reason;
        BanImposerId = bannedBy;
        BannedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public static Result<UserBan> Create(Guid userId, string reason, Guid bannedBy, DateTime expiresAt)
    {
        UserBan userBan = new UserBan(Guid.NewGuid(), userId, reason, bannedBy, expiresAt);

        return Result.Success(userBan);
    }

    public void Deactivate(Guid banRemoverId)
    {
        UnbannedAt = DateTime.UtcNow;
        BanRemoverId = banRemoverId;
    }

    public bool IsCurrentlyActive()
    {
        return !UnbannedAt.HasValue && ExpiresAt > DateTime.UtcNow;
    }
}
