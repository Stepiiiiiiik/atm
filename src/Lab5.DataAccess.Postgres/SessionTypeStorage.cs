using Dapper;
using Lab5.Core.Abstractions;
using System.Data;

namespace Lab5.DataAccess.Postgres;

public class SessionTypeStorage : ISessionTypeStorage
{
    private readonly DapperContext _dapperContext;

    private readonly Dictionary<string, int> _cache = [];

    public SessionTypeStorage(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<int?> GetSessionTypeIdByName(string sessionTypeName)
    {
        if (_cache.TryGetValue(sessionTypeName, out int value))
        {
            return value;
        }

        const string query = """
                             SELECT session_type_id
                             FROM session_type
                             WHERE session_type_name = @sessionTypeName
                             """;

        var parameters = new DynamicParameters();

        parameters.Add("sessionTypeName", sessionTypeName);

        using IDbConnection connection = _dapperContext.CreateConnection();

        int? result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

        if (result != null)
        {
            _cache[sessionTypeName] = (int)result;
        }

        return result;
    }
}