namespace Shared.Contracts;

public record AchievementAwarded(
    Guid UserId,
    Guid EventId,
    string AchievementCode,
    string AchievementName,
    string AchievementDescription,
    DateTime AwardedAtUtc
);
