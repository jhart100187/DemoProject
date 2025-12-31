using Npgsql;
using Microsoft.Extensions.Configuration;

namespace DemoProject.Domain;
public interface INpgsqlConnectionFactory
{
    Task<NpgsqlConnection> CreateConnectionAsync();
}

public sealed class NpgsqlConnectionFactory : INpgsqlConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Postgres")!;
    }

    public async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }
}
