using HugSpace.Domain.Common;

namespace HugSpace.Domain.Entities;

public enum FollowStatus
{
    Pending = 0,
    Accepted = 1,
    Blocked = 2
}

public class Follow : BaseEntity
{
    public Guid FollowerId { get; private set; }
    public Guid FollowingId { get; private set; }
    public FollowStatus Status { get; private set; }

    // Propriedades de navegação
    public User? Follower { get; private set; }
    public User? Following { get; private set; }

    public Follow() { }

    public Follow(Guid followerId, Guid followingId, bool requiresApproval = false)
    {
        FollowerId = followerId;
        FollowingId = followingId;
        Status = requiresApproval ? FollowStatus.Pending : FollowStatus.Accepted;
    }

    public void Accept()
    {
        if (Status == FollowStatus.Pending)
        {
            Status = FollowStatus.Accepted;
            UpdateTimestamp();
        }
    }

    public void Block()
    {
        Status = FollowStatus.Blocked;
        UpdateTimestamp();
    }

    public void Unblock()
    {
        if (Status == FollowStatus.Blocked)
        {
            Status = FollowStatus.Accepted;
            UpdateTimestamp();
        }
    }
}