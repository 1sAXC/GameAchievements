namespace AuthService.Contracts;

public sealed record AuthTokenResponse(string AccessToken, DateTime ExpiresAtUtc);
