using Dapper;
using Lab5.Core.Abstractions;
using Lab5.Core.Entities;
using System.Data;

namespace Lab5.DataAccess.Postgres;

public class SessionStorage : ISessionStorage
{
    private readonly DapperContext _dapperContext;

    public SessionStorage(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<Session?> FindSession(Guid sessionKey)
    {
        const string query = """
                             SELECT *
                             FROM session
                             WHERE session_key = @sessionKey;
                             """;

        var parameters = new DynamicParameters();
        parameters.Add("sessionKey", sessionKey);

        using IDbConnection connection = _dapperContext.CreateConnection();

        Session? result = await connection.QuerySingleOrDefaultAsync<Session>(query, parameters);
        return result;
    }

    public async Task<OperationResult<Guid>> AddSession(Session session)
    {
        const string query = """
                             INSERT INTO session (session_key, session_type_id, account_id)
                             VALUES (@sessionKey, @sessionTypeId, @accountId);
                             """;

        var parameters = new DynamicParameters();
        parameters.Add("sessionKey", session.SessionKey);
        parameters.Add("sessionTypeId", session.SessionTypeId);
        parameters.Add("accountId", session.AccountId);

        using IDbConnection connection = _dapperContext.CreateConnection();

        await connection.ExecuteAsync(query, parameters);

        return new OperationResult<Guid>.Success(session.SessionKey);
    }
}