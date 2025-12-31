
using Dapper;
using DemoProject.Domain.Models.Users;

namespace DemoProject.Domain.Repositories.Users;

public interface IUserAddressRepository
{
    Task<Guid> CreateAsync(
        Guid userId,
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string postalCode,
        string country = "US");

    Task<UserAddress?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<UserAddress>> GetByUserIdAsync(Guid userId);

    Task<bool> UpdateAsync(UserAddress address);

    Task<bool> DeleteAsync(Guid id);
}

public sealed class UserAddressRepository : IUserAddressRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public UserAddressRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(
        Guid userId,
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string postalCode,
        string country = "US")
    {
        const string sql = """
            INSERT INTO demoproject.users_addresses (
                id,
                user_id,
                address_line1,
                address_line2,
                city,
                state,
                postal_code,
                country
            )
            VALUES (
                uuid_generate_v4(),
                @UserId,
                @AddressLine1,
                @AddressLine2,
                @City,
                @State,
                @PostalCode,
                @Country
            )
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            UserId = userId,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country
        });
    }

    public async Task<UserAddress?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                address_line1 AS AddressLine1,
                address_line2 AS AddressLine2,
                city,
                state,
                postal_code AS PostalCode,
                country,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.users_addresses
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<UserAddress>(sql, new { Id = id });
    }

    public async Task<IReadOnlyList<UserAddress>> GetByUserIdAsync(Guid userId)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                address_line1 AS AddressLine1,
                address_line2 AS AddressLine2,
                city,
                state,
                postal_code AS PostalCode,
                country,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM demoproject.users_addresses
            WHERE user_id = @UserId
            ORDER BY created_at;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var result = await conn.QueryAsync<UserAddress>(sql, new { UserId = userId });
        return result.AsList();
    }

    public async Task<bool> UpdateAsync(UserAddress address)
    {
        const string sql = """
            UPDATE demoproject.users_addresses
            SET
                address_line1 = @AddressLine1,
                address_line2 = @AddressLine2,
                city = @City,
                state = @State,
                postal_code = @PostalCode,
                country = @Country,
                updated_at = now()
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new
        {
            address.Id,
            address.AddressLine1,
            address.AddressLine2,
            address.City,
            address.State,
            address.PostalCode,
            address.Country
        });

        return rows == 1;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM demoproject.users_addresses
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = id });

        return rows == 1;
    }
}
