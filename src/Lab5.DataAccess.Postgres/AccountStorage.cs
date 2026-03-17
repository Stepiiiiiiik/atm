using Dapper;
using Lab5.Core.Abstractions;
using Lab5.Core.Entities;
using System.Data;

namespace Lab5.DataAccess.Postgres;

public class AccountStorage : IAccountStorage
{
    private readonly DapperContext _dapperContext;

    public AccountStorage(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task AddAccount(Account account)
    {
        const string query = """
                             INSERT INTO account (account_id, pin_hash)
                             VALUES (@accountId, @pinHash);
                             """;
        var parameters = new DynamicParameters();
        parameters.Add("accountId", account.AccountId);
        parameters.Add("pinHash", account.PinHash);

        using IDbConnection connection = _dapperContext.CreateConnection();
        await connection.ExecuteAsync(query, parameters);
    }

    public async Task<Account?> FindAccount(Guid accountId)
    {
        const string query = """
                             SELECT * 
                             FROM account
                             WHERE account_id = @accountId;
                             """;

        var parameters = new DynamicParameters();
        parameters.Add("accountId", accountId);

        using IDbConnection connection = _dapperContext.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Account>(query, parameters);
    }

    public async Task UpdateAccount(Account account)
    {
        const string query = """
                             UPDATE account
                             SET  pin_hash = @pinHash, balance = @balance
                             WHERE account_id = @accountId;
                             """;

        var parameters = new DynamicParameters();
        parameters.Add("accountId", account.AccountId);
        parameters.Add("pinHash", account.PinHash);
        parameters.Add("balance", account.Balance);

        using IDbConnection connection = _dapperContext.CreateConnection();

        await connection.ExecuteAsync(query, parameters);
    }
}