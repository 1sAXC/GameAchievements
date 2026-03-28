namespace AchievementsService.Entities;

public sealed class UserStat
{
    public Guid UserId { get; set; }
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int TotalScore { get; set; }
    public int TotalKills { get; set; }
}
