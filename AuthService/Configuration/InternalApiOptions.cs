namespace AuthService.Configuration;

public sealed class InternalApiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string HeaderName { get; init; } = "X-Internal-Api-Key";
}
