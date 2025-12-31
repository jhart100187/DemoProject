
namespace DemoProject.Domain.Models.Users;

public sealed record UserSession(
    Guid Id,
    Guid UserId,
    string SessionTokenHash,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? LastActivityAt,
    string? IpAddress,
    string? UserAgent,
    bool IsRevoked
);