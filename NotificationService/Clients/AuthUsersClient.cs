using System.Net;
using System.Net.Http.Json;
using NotificationService.Configuration;

namespace NotificationService.Clients;

public sealed class AuthUsersClient(HttpClient httpClient)
{
    public async Task<string> GetUserEmailAsync(Guid userId, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync($"internal/users/{userId}/email", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"User {userId} was not found in AuthService.");
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<InternalUserEmailResponse>(cancellationToken: cancellationToken);
        if (payload is null || string.IsNullOrWhiteSpace(payload.Email))
        {
            throw new InvalidOperationException($"AuthService returned an empty email for user {userId}.");
        }

        return payload.Email;
    }

    public static void ConfigureHttpClient(HttpClient client, AuthServiceOptions options)
    {
        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException("AuthService:BaseUrl is missing or invalid.");
        }

        if (string.IsNullOrWhiteSpace(options.InternalApiKey))
        {
            throw new InvalidOperationException("AuthService:InternalApiKey is missing.");
        }

        if (string.IsNullOrWhiteSpace(options.InternalApiKeyHeaderName))
        {
            throw new InvalidOperationException("AuthService:InternalApiKeyHeaderName is missing.");
        }

        client.BaseAddress = baseUri;
        client.Timeout = TimeSpan.FromSeconds(3);
        client.DefaultRequestHeaders.Remove(options.InternalApiKeyHeaderName);
        client.DefaultRequestHeaders.Add(options.InternalApiKeyHeaderName, options.InternalApiKey);
    }

    private sealed record InternalUserEmailResponse(Guid UserId, string Email);
}
