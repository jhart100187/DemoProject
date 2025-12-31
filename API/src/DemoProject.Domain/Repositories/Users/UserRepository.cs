
using Dapper;
using DemoProject.Domain.Models.Users;

namespace DemoProject.Domain.Repositories.Users;

public interface IUserRepository
{
    Task<Guid> CreateAsync(
        string firstName,
        string lastName,
        string email);
        
    Task<User?> GetByIdAsync(Guid id);

    Task<bool> UpdateAsync(User user);

    Task<bool> DeleteAsync(Guid id);

    Task UpsertPasswordAsync(Guid userId, string passwordHash);
}

public sealed class UserRepository : IUserRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public UserRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(
        string firstName,
        string lastName,
        string email)
    {
        const string sql = """
            INSERT INTO demoproject.users (firstname, lastname, email)
            VALUES (@FirstName, @LastName, @Email)
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email
        });
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT id, firstname, lastname, email
            FROM demoproject.users
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string sql = """
            UPDATE demoproject.users
            SET
                firstname = @FirstName,
                lastname = @LastName,
                email = @Email
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email
        });

        return rows == 1;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM demoproject.users
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = id });

        return rows == 1;
    }

    public async Task UpsertPasswordAsync(Guid userId, string passwordHash)
    {
        const string sql = """
            INSERT INTO demoproject.users_passwords (user_id, password_hash)
            VALUES (@UserId, @PasswordHash)
            ON CONFLICT (user_id)
            DO UPDATE SET
                password_hash = EXCLUDED.password_hash,
                created_at = now();
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        await conn.ExecuteAsync(sql, new
        {
            UserId = userId,
            PasswordHash = passwordHash
        });
    }
}