
namespace DemoProject.Domain.Models.ShoppingCarts;

public sealed record ShoppingCartItem(
    Guid Id,
    Guid ShoppingCartId,
    Guid ProductId,
    int Quantity,
    decimal PriceAtAdd,
    string Currency,
    DateTimeOffset AddedAt
);
