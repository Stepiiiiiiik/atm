using Lab5.Core.Entities;

namespace Lab5.Core.Abstractions;

public interface IAccountService
{
    Task<OperationResult<Guid>> CreateAccount(string pin, Guid sessionKey);

    Task<OperationResult<bool>> CheckAccountData(string pin, Guid accountId);

    Task<OperationResult<decimal>> Deposit(Guid accountId, decimal amount, Guid sessionKey);

    Task<OperationResult<decimal>> Withdraw(Guid accountId, decimal amount, Guid sessionKey);

    Task<OperationResult<decimal>> CheckBalance(Guid accountId, Guid sessionKey);

    Task<OperationResult<IReadOnlyCollection<HistoryItem>>> GetHistory(Guid accountId, Guid sessionKey);
}