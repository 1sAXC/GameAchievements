using System.Net.Http.Json;
using ResultsService.Configuration;

namespace ResultsService.Clients;

public sealed class AuthUsersClient(HttpClient httpClient)
{
    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<UserExistsResponse>($"auth/users/{userId}/exists", cancellationToken);
        return response?.Exists ?? false;
    }

    public static void ConfigureHttpClient(HttpClient client, AuthServiceOptions options)
    {
        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException("AuthService:BaseUrl is missing or invalid.");
        }

        client.BaseAddress = baseUri;
        client.Timeout = TimeSpan.FromSeconds(3);
    }

    private sealed record UserExistsResponse(Guid UserId, bool Exists);
}
