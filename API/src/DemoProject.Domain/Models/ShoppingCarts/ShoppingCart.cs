
namespace DemoProject.Domain.Models.ShoppingCarts;

public sealed record ShoppingCart(
    Guid Id,
    Guid UserId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
