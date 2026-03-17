using Dapper;
using Lab5.Core.Abstractions;
using Lab5.Core.Entities;
using System.Data;

namespace Lab5.DataAccess.Postgres;

public class HistoryStorage : IHistoryStorage
{
    private readonly DapperContext _dapperContext;

    public HistoryStorage(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task AddHistoryItem(HistoryItem historyItem)
    {
        const string query = """
                             INSERT INTO history_item (history_item_id ,account_id, operation_type_id, value, session_key, success)
                             VALUES (@historyItemId ,@account_id, @operation_type_id, @value, @session_key, @success);
                             """;

        var parameters = new DynamicParameters();
        parameters.Add("@historyItemId", historyItem.HistoryItemId);
        parameters.Add("@account_id", historyItem.AccountId);
        parameters.Add("@operation_type_id", historyItem.OperationTypeId);
        parameters.Add("@value", historyItem.Value);
        parameters.Add("@session_key", historyItem.SessionKey);
        parameters.Add("@success", historyItem.Success);

        using IDbConnection connection = _dapperContext.CreateConnection();

        await connection.ExecuteAsync(query, parameters);
    }

    public async Task<IReadOnlyCollection<HistoryItem>> GetHistoryByAccountId(Guid accountId)
    {
        const string query = """
                             SELECT *
                             FROM  history_item
                             WHERE account_id = @accountId
                             ORDER BY created_at DESC;
                             """;

        var parameters = new DynamicParameters();
        parameters.Add("@accountId", accountId);

        using IDbConnection connection = _dapperContext.CreateConnection();

        IEnumerable<HistoryItem> historyItems = await connection.QueryAsync<HistoryItem>(query, parameters);

        return historyItems.ToList();
    }
}