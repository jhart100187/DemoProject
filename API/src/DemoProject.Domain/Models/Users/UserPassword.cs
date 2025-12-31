
namespace DemoProject.Domain.Models.Users;
public sealed record UserPassword(
    Guid UserId,
    string PasswordHash,
    DateTimeOffset CreatedAt
);