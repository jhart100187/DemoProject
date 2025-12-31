
namespace DemoProject.Domain.Models.Users;

public sealed record UserAddress(
    Guid Id,
    Guid UserId,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
