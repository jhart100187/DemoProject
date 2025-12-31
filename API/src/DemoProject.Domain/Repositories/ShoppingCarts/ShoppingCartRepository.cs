
using Dapper;
using DemoProject.Domain.Models.ShoppingCarts;

namespace DemoProject.Domain.Repositories.ShoppingCarts;

public interface IShoppingCartRepository
{
    // Cart operations
    Task<Guid> CreateCartAsync(Guid userId);
    Task<ShoppingCart?> GetCartByIdAsync(Guid cartId);
    Task<ShoppingCart?> GetCartByUserIdAsync(Guid userId);
    Task<bool> UpdateCartTimestampAsync(Guid cartId);

    Task<bool> DeleteCartAsync(Guid cartId);

    // Cart item operations
    Task<Guid> AddItemAsync(
        Guid cartId,
        Guid productId,
        int quantity,
        decimal priceAtAdd,
        string currency);

    Task<IReadOnlyList<ShoppingCartItem>> GetItemsAsync(Guid cartId);

    Task<bool> UpdateItemQuantityAsync(Guid itemId, int newQuantity);

    Task<bool> RemoveItemAsync(Guid itemId);

    Task<bool> ClearCartAsync(Guid cartId);
}

public sealed class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ShoppingCartRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateCartAsync(Guid userId)
    {
        const string sql = """
            INSERT INTO demoproject.shopping_carts (
                id,
                user_id
            )
            VALUES (
                uuid_generate_v4(),
                @UserId
            )
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new { UserId = userId });
    }

    public async Task<ShoppingCart?> GetCartByIdAsync(Guid cartId)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.shopping_carts
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<ShoppingCart>(sql, new { Id = cartId });
    }

    public async Task<ShoppingCart?> GetCartByUserIdAsync(Guid userId)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.shopping_carts
            WHERE user_id = @UserId;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<ShoppingCart>(sql, new { UserId = userId });
    }

    public async Task<bool> UpdateCartTimestampAsync(Guid cartId)
    {
        const string sql = """
            UPDATE demoproject.shopping_carts
            SET updated_at = now()
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = cartId });
        return rows == 1;
    }

    public async Task<bool> DeleteCartAsync(Guid cartId)
    {
        const string sql = """
            DELETE FROM demoproject.shopping_carts
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = cartId });
        return rows == 1;
    }

    public async Task<Guid> AddItemAsync(
        Guid cartId,
        Guid productId,
        int quantity,
        decimal priceAtAdd,
        string currency)
    {
        const string sql = """
            INSERT INTO demoproject.shopping_carts_items (
                id,
                shopping_cart_id,
                product_id,
                quantity,
                price_at_add,
                currency
            )
            VALUES (
                uuid_generate_v4(),
                @CartId,
                @ProductId,
                @Quantity,
                @PriceAtAdd,
                @Currency
            )
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            CartId = cartId,
            ProductId = productId,
            Quantity = quantity,
            PriceAtAdd = priceAtAdd,
            Currency = currency
        });
    }

    public async Task<IReadOnlyList<ShoppingCartItem>> GetItemsAsync(Guid cartId)
    {
        const string sql = """
            SELECT
                id,
                shopping_cart_id AS ShoppingCartId,
                product_id AS ProductId,
                quantity,
                price_at_add AS PriceAtAdd,
                currency,
                added_at AS AddedAt
            FROM demoproject.shopping_carts_items
            WHERE shopping_cart_id = @CartId
            ORDER BY added_at;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var items = await conn.QueryAsync<ShoppingCartItem>(sql, new { CartId = cartId });
        return items.AsList();
    }

    public async Task<bool> UpdateItemQuantityAsync(Guid itemId, int newQuantity)
    {
        const string sql = """
            UPDATE demoproject.shopping_carts_items
            SET quantity = @Quantity
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = itemId, Quantity = newQuantity });
        return rows == 1;
    }

    public async Task<bool> RemoveItemAsync(Guid itemId)
    {
        const string sql = """
            DELETE FROM demoproject.shopping_carts_items
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = itemId });
        return rows == 1;
    }

    public async Task<bool> ClearCartAsync(Guid cartId)
    {
        const string sql = """
            DELETE FROM demoproject.shopping_carts_items
            WHERE shopping_cart_id = @CartId;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { CartId = cartId });
        return rows >= 0; // even if zero rows, it's successful
    }
}
