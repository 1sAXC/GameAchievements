namespace ResultsService.Contracts;

public sealed record ResultResponse(
    Guid Id,
    Guid UserId,
    string GameId,
    int Score,
    int Kills,
    int Deaths,
    bool IsWin,
    bool IsPerfect,
    DateTime PlayedAt,
    DateTime CreatedAtUtc
);
