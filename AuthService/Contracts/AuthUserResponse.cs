namespace AuthService.Contracts;

public sealed record AuthUserResponse(Guid Id, string Email, DateTime CreatedAtUtc);
