namespace AuthService.Contracts;

public sealed record UserExistsResponse(Guid UserId, bool Exists);
