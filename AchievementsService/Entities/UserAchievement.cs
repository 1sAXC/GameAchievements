namespace AchievementsService.Entities;

public sealed class UserAchievement
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AchievementCode { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }

    public Achievement Achievement { get; set; } = null!;
}
