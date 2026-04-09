namespace AuthService.Contracts;

public sealed record InternalUserEmailResponse(
    Guid UserId,
    string Email
);
