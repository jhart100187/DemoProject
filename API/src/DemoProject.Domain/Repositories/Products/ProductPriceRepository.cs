
using Dapper;
using DemoProject.Domain.Models.Products;

namespace DemoProject.Domain.Repositories.Products;

public interface IProductPriceRepository
{
    Task<Guid> CreateAsync(
        Guid productId,
        decimal price,
        string currency,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo = null);

    Task<ProductPrice?> GetByIdAsync(Guid id);

    Task<ProductPrice?> GetCurrentAsync(Guid productId, DateTimeOffset asOf);

    Task<IReadOnlyList<ProductPrice>> GetHistoryAsync(Guid productId);

    Task<bool> CloseAsync(Guid priceId, DateTimeOffset effectiveTo);
}

public sealed class ProductPriceRepository : IProductPriceRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ProductPriceRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(
        Guid productId,
        decimal price,
        string currency,
        DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo = null)
    {
        const string sql = """
            INSERT INTO demoproject.products_prices (
                id,
                product_id,
                price,
                currency,
                effective_from,
                effective_to
            )
            VALUES (
                uuid_generate_v4(),
                @ProductId,
                @Price,
                @Currency,
                @EffectiveFrom,
                @EffectiveTo
            )
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            ProductId = productId,
            Price = price,
            Currency = currency,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo
        });
    }

    public async Task<ProductPrice?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id,
                product_id AS ProductId,
                price,
                currency,
                effective_from AS EffectiveFrom,
                effective_to AS EffectiveTo
            FROM demoproject.products_prices
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<ProductPrice>(sql, new { Id = id });
    }

    public async Task<ProductPrice?> GetCurrentAsync(Guid productId, DateTimeOffset asOf)
    {
        const string sql = """
            SELECT
                id,
                product_id AS ProductId,
                price,
                currency,
                effective_from AS EffectiveFrom,
                effective_to AS EffectiveTo
            FROM demoproject.products_prices
            WHERE product_id = @ProductId
              AND effective_from <= @AsOf
              AND (effective_to IS NULL OR effective_to > @AsOf)
            ORDER BY effective_from DESC
            LIMIT 1;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<ProductPrice>(sql, new
        {
            ProductId = productId,
            AsOf = asOf
        });
    }

    public async Task<IReadOnlyList<ProductPrice>> GetHistoryAsync(Guid productId)
    {
        const string sql = """
            SELECT
                id,
                product_id AS ProductId,
                price,
                currency,
                effective_from AS EffectiveFrom,
                effective_to AS EffectiveTo
            FROM demoproject.products_prices
            WHERE product_id = @ProductId
            ORDER BY effective_from DESC;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var result = await conn.QueryAsync<ProductPrice>(sql, new
        {
            ProductId = productId
        });

        return result.AsList();
    }

    public async Task<bool> CloseAsync(Guid priceId, DateTimeOffset effectiveTo)
    {
        const string sql = """
            UPDATE demoproject.products_prices
            SET effective_to = @EffectiveTo
            WHERE id = @Id
              AND effective_to IS NULL;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new
        {
            Id = priceId,
            EffectiveTo = effectiveTo
        });

        return rows == 1;
    }
}
