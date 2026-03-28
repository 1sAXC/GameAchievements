namespace Shared.Contracts;

public record CreateResultRequest(
    string GameId,
    int Score,
    int Kills,
    int Deaths,
    bool Win,
    DateTime PlayedAt
);
