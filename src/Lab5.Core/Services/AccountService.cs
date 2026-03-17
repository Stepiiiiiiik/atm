using Lab5.Core.Abstractions;
using Lab5.Core.Entities;

namespace Lab5.Core.Services;

public class AccountService : IAccountService
{
    private readonly IAccountStorage _accountStorage;
    private readonly IHistoryStorage _historyStorage;
    private readonly IOperationTypeStorage _operationTypeStorage;
    private readonly IPinHashService _pinHashService;

    public AccountService(
        IAccountStorage accountStorage,
        IHistoryStorage historyStorage,
        IOperationTypeStorage operationTypeStorage,
        IPinHashService pinHashService)
    {
        _accountStorage = accountStorage;
        _historyStorage = historyStorage;
        _operationTypeStorage = operationTypeStorage;
        _pinHashService = pinHashService;
    }

    public async Task<OperationResult<Guid>> CreateAccount(string pin, Guid sessionKey)
    {
        bool isValidPin = pin.Length == 4;

        foreach (char character in pin.Where(character => !char.IsDigit(character)))
        {
            isValidPin = false;
        }

        if (!isValidPin)
        {
            return new OperationResult<Guid>.BadRequest("Invalid pin format");
        }

        string pinHash = _pinHashService.GetHash(pin);

        var account = new Account(pinHash);

        await _accountStorage.AddAccount(account);

        int? operationTypeId = await _operationTypeStorage.GetOperationTypeIdByName("CREATE");
        if (operationTypeId == null)
        {
            return new OperationResult<Guid>.InternalError("Internal server database error");
        }

        var historyItem = new HistoryItem
        {
            OperationTypeId = operationTypeId.Value,
            AccountId = account.AccountId,
            SessionKey = sessionKey,
            Success = true,
        };

        await _historyStorage.AddHistoryItem(historyItem);

        return new OperationResult<Guid>.Success(account.AccountId);
    }

    public async Task<OperationResult<bool>> CheckAccountData(string pin, Guid accountId)
    {
        Account? account = await _accountStorage.FindAccount(accountId);
        return account == null
            ? new OperationResult<bool>.BadRequest("Account not found")
            : _pinHashService.VerifyPin(pin, account.PinHash)
                ? new OperationResult<bool>.Success(true)
                : new OperationResult<bool>.BadRequest("Incorrect pin");
    }

    public async Task<OperationResult<decimal>> Deposit(Guid accountId, decimal amount, Guid sessionKey)
    {
        int? operationTypeId = await _operationTypeStorage.GetOperationTypeIdByName("DEPOSIT");
        if (operationTypeId == null)
        {
            return new OperationResult<decimal>.InternalError("Internal server database error");
        }

        var historyItem = new HistoryItem
        {
            OperationTypeId = operationTypeId.Value,
            SessionKey = sessionKey,
            Value = amount,
        };

        Account? account = await _accountStorage.FindAccount(accountId);
        if (account == null)
        {
            return new OperationResult<decimal>.BadRequest("Account not found");
        }

        historyItem.AccountId = account.AccountId;

        if (amount <= 0)
        {
            historyItem.Success = false;
            await _historyStorage.AddHistoryItem(historyItem);

            return new OperationResult<decimal>.BadRequest("Amount must be greater than zero");
        }

        account.Balance += amount;
        await _accountStorage.UpdateAccount(account);

        historyItem.Success = true;
        await _historyStorage.AddHistoryItem(historyItem);

        return new OperationResult<decimal>.Success(account.Balance);
    }

    public async Task<OperationResult<decimal>> Withdraw(Guid accountId, decimal amount, Guid sessionKey)
    {
        int? operationTypeId = await _operationTypeStorage.GetOperationTypeIdByName("WITHDRAW");
        if (operationTypeId == null)
        {
            return new OperationResult<decimal>.InternalError("Internal server database error");
        }

        var historyItem = new HistoryItem
        {
            OperationTypeId = operationTypeId.Value,
            SessionKey = sessionKey,
            Value = amount,
        };

        Account? account = await _accountStorage.FindAccount(accountId);
        if (account == null)
        {
            return new OperationResult<decimal>.BadRequest("Account not found");
        }

        historyItem.AccountId = account.AccountId;

        if (amount <= 0)
        {
            historyItem.Success = false;
            await _historyStorage.AddHistoryItem(historyItem);

            return new OperationResult<decimal>.BadRequest("Amount must be greater than zero");
        }

        if (account.Balance - amount < 0)
        {
            historyItem.Success = false;
            await _historyStorage.AddHistoryItem(historyItem);

            return new OperationResult<decimal>.BadRequest("Insufficient funds");
        }

        account.Balance -= amount;
        await _accountStorage.UpdateAccount(account);

        historyItem.Success = true;
        await _historyStorage.AddHistoryItem(historyItem);

        return new OperationResult<decimal>.Success(account.Balance);
    }

    public async Task<OperationResult<decimal>> CheckBalance(Guid accountId, Guid sessionKey)
    {
        int? operationTypeId = await _operationTypeStorage.GetOperationTypeIdByName("BALANCE");
        if (operationTypeId == null)
        {
            return new OperationResult<decimal>.InternalError("Internal server database error");
        }

        var historyItem = new HistoryItem
        {
            OperationTypeId = operationTypeId.Value,
            SessionKey = sessionKey,
        };

        Account? account = await _accountStorage.FindAccount(accountId);
        if (account == null)
        {
            return new OperationResult<decimal>.BadRequest("Account not found");
        }

        historyItem.AccountId = account.AccountId;
        historyItem.Success = true;
        await _historyStorage.AddHistoryItem(historyItem);

        return new OperationResult<decimal>.Success(account.Balance);
    }

    public async Task<OperationResult<IReadOnlyCollection<HistoryItem>>> GetHistory(Guid accountId, Guid sessionKey)
    {
        int? operationTypeId = await _operationTypeStorage.GetOperationTypeIdByName("HISTORY");
        if (operationTypeId == null)
        {
            return new OperationResult<IReadOnlyCollection<HistoryItem>>.InternalError(
                "Internal server database error");
        }

        var historyItem = new HistoryItem
        {
            OperationTypeId = operationTypeId.Value,
            SessionKey = sessionKey,
        };

        Account? account = await _accountStorage.FindAccount(accountId);
        if (account == null)
        {
            return new OperationResult<IReadOnlyCollection<HistoryItem>>.BadRequest("Account not found");
        }

        historyItem.AccountId = account.AccountId;

        IReadOnlyCollection<HistoryItem> result = await _historyStorage.GetHistoryByAccountId(accountId);

        historyItem.Success = true;
        await _historyStorage.AddHistoryItem(historyItem);

        return new OperationResult<IReadOnlyCollection<HistoryItem>>.Success(result);
    }
}