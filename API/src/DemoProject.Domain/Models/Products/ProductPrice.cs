
namespace DemoProject.Domain.Models.Products;

public sealed record ProductPrice(
    Guid Id,
    Guid ProductId,
    decimal Price,
    string Currency,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveTo
);
