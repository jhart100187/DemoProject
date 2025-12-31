
namespace DemoProject.Domain.Models.Products;

public sealed record Product(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
