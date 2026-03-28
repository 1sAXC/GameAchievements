namespace AchievementsService.Contracts;

public sealed record AchievementDefinitionResponse(
    string Code,
    string Name,
    string Description,
    string Type,
    int? TargetValue
);
