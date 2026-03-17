using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace Lab5.DataAccess.Postgres;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IOptions<DatabaseSettings> options)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        _connectionString = options.Value.ConnectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}