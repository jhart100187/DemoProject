
using Dapper;
using DemoProject.Domain.Models.Users;

namespace DemoProject.Domain.Repositories.Users;

public interface IUserSessionRepository
{
    Task<Guid> CreateAsync(
        Guid userId,
        string sessionTokenHash,
        DateTimeOffset expiresAt,
        string? ipAddress,
        string? userAgent);

    Task<UserSession?> GetByIdAsync(Guid id);

    Task<UserSession?> GetByTokenHashAsync(string sessionTokenHash);

    Task<bool> RevokeAsync(Guid sessionId);

    Task<bool> UpdateLastActivityAsync(Guid sessionId);

    Task<int> RevokeAllForUserAsync(Guid userId);
}

public sealed class UserSessionRepository : IUserSessionRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public UserSessionRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(
        Guid userId,
        string sessionTokenHash,
        DateTimeOffset expiresAt,
        string? ipAddress,
        string? userAgent)
    {
        const string sql = """
            INSERT INTO demoproject.users_sessions (
                id,
                user_id,
                session_token_hash,
                expires_at,
                ip_address,
                user_agent
            )
            VALUES (
                uuid_generate_v4(),
                @UserId,
                @SessionTokenHash,
                @ExpiresAt,
                @IpAddress,
                @UserAgent
            )
            RETURNING id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteScalarAsync<Guid>(sql, new
        {
            UserId = userId,
            SessionTokenHash = sessionTokenHash,
            ExpiresAt = expiresAt,
            IpAddress = ipAddress,
            UserAgent = userAgent
        });
    }

    public async Task<UserSession?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                session_token_hash AS SessionTokenHash,
                created_at AS CreatedAt,
                expires_at AS ExpiresAt,
                last_activity_at AS LastActivityAt,
                ip_address AS IpAddress,
                user_agent AS UserAgent,
                is_revoked AS IsRevoked
            FROM demoproject.users_sessions
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<UserSession>(sql, new { Id = id });
    }

    public async Task<UserSession?> GetByTokenHashAsync(string sessionTokenHash)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                session_token_hash AS SessionTokenHash,
                created_at AS CreatedAt,
                expires_at AS ExpiresAt,
                last_activity_at AS LastActivityAt,
                ip_address AS IpAddress,
                user_agent AS UserAgent,
                is_revoked AS IsRevoked
            FROM demoproject.users_sessions
            WHERE session_token_hash = @SessionTokenHash;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.QuerySingleOrDefaultAsync<UserSession>(sql, new
        {
            SessionTokenHash = sessionTokenHash
        });
    }

    public async Task<bool> RevokeAsync(Guid sessionId)
    {
        const string sql = """
            UPDATE demoproject.users_sessions
            SET is_revoked = true
            WHERE id = @Id;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = sessionId });

        return rows == 1;
    }

    public async Task<bool> UpdateLastActivityAsync(Guid sessionId)
    {
        const string sql = """
            UPDATE demoproject.users_sessions
            SET last_activity_at = now()
            WHERE id = @Id
              AND is_revoked = false
              AND expires_at > now();
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        var rows = await conn.ExecuteAsync(sql, new { Id = sessionId });

        return rows == 1;
    }

    public async Task<int> RevokeAllForUserAsync(Guid userId)
    {
        const string sql = """
            UPDATE demoproject.users_sessions
            SET is_revoked = true
            WHERE user_id = @UserId
              AND is_revoked = false;
            """;

        await using var conn = await _connectionFactory.CreateConnectionAsync();

        return await conn.ExecuteAsync(sql, new { UserId = userId });
    }
}
