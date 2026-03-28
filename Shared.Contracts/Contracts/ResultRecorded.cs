namespace Shared.Contracts;

public record ResultRecorded(
    Guid ResultId,
    Guid UserId,
    string GameId,
    int Score,
    int Kills,
    int Deaths,
    bool Win,
    bool IsPerfect,
    DateTime PlayedAt
);
