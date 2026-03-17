using Dapper;
using Lab5.Core.Abstractions;
using System.Data;

namespace Lab5.DataAccess.Postgres;

public class OperationTypeStorage : IOperationTypeStorage
{
    private readonly DapperContext _dapperContext;

    private readonly Dictionary<string, int> _cache = [];

    public OperationTypeStorage(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<int?> GetOperationTypeIdByName(string operationName)
    {
        if (_cache.TryGetValue(operationName, out int value))
        {
            return value;
        }

        const string query = """
                             SELECT operation_type_id
                             FROM operation_type
                             WHERE operation_name = @operationName
                             """;

        var parameters = new DynamicParameters();

        parameters.Add("operationName", operationName);

        using IDbConnection connection = _dapperContext.CreateConnection();

        int? result = await connection.QueryFirstOrDefaultAsync<int?>(query, parameters);

        if (result != null)
        {
            _cache[operationName] = (int)result;
        }

        return result;
    }
}