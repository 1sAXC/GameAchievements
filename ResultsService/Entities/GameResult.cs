namespace ResultsService.Entities;

public sealed class GameResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string GameId { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public bool IsWin { get; set; }
    public bool IsPerfect { get; set; }
    public DateTime PlayedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
