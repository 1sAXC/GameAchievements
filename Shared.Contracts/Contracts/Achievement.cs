namespace Shared.Contracts;

public record Achievement(
    string Code,
    string Name,
    string Description,
    DateTime GrantedAt
);