namespace AchievementsService.Entities;

public sealed class Achievement
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int? TargetValue { get; set; }
}
