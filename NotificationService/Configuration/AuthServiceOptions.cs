namespace NotificationService.Configuration;

public sealed class AuthServiceOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string InternalApiKey { get; init; } = string.Empty;
    public string InternalApiKeyHeaderName { get; init; } = "X-Internal-Api-Key";
}
