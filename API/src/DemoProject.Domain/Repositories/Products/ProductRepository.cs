
using Dapper;
using DemoProject.Domain.Models.Products;

namespace DemoProject.Domain.Repositories.Products;

public interface IProductRepository
{
    Task<Guid> CreateAsync(
        string sku,
        string name,
        string? description,
        bool isActive = true);

    Task<Product?> GetByIdAsync(Guid id);

    Task<Product?> GetBySkuAsync(string sku);

    Task<IReadOnlyList<Product>> GetActiveAsync();

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeactivateAsync(Guid id);
}

public sealed class ProductRepository : IProductRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ProductRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(
        string sku,
        string name,
        string? description,
        bool isActive = true)
    {
        const string sql = """
            INSERT INTO demoproject.products (
                id,
                sku,
                name,
                description,
                is_active
            )
            VALUES (
                uuid_generate_v4(),
                @Sku,
                @Name,
                @Description,
                @IsActive
            )
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            Sku = sku,
            Name = name,
            Description = description,
            IsActive = isActive
        });
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id,
                sku,
                name,
                description,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.products
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        const string sql = """
            SELECT
                id,
                sku,
                name,
                description,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.products
            WHERE sku = @Sku;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<Product>(sql, new { Sku = sku });
    }

    public async Task<IReadOnlyList<Product>> GetActiveAsync()
    {
        const string sql = """
            SELECT
                id,
                sku,
                name,
                description,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.products
            WHERE is_active = true
            ORDER BY name;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var result = await conn.QueryAsync<Product>(sql);
        return result.AsList();
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        const string sql = """
            UPDATE demoproject.products
            SET
                sku = @Sku,
                name = @Name,
                description = @Description,
                is_active = @IsActive,
                updated_at = now()
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new
        {
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.IsActive
        });

        return rows == 1;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        const string sql = """
            UPDATE demoproject.products
            SET
                is_active = false,
                updated_at = now()
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = id });

        return rows == 1;
    }
}
