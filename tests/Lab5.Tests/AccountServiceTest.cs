using Lab5.Core.Abstractions;
using Lab5.Core.Entities;
using Lab5.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab5.Tests;

public class AccountServiceTest
{
    private readonly IAccountStorage _accountStorage;
    private readonly IHistoryStorage _historyStorage;
    private readonly IOperationTypeStorage _operationTypeStorage;
    private readonly AccountService _service;

    public AccountServiceTest()
    {
        _accountStorage = Substitute.For<IAccountStorage>();
        _historyStorage = Substitute.For<IHistoryStorage>();
        _operationTypeStorage = Substitute.For<IOperationTypeStorage>();
        PinHashService pinHashServiceMock = Substitute.For<PinHashService>();

        var services = new ServiceCollection();

        services.AddSingleton(_accountStorage);
        services.AddSingleton(_historyStorage);
        services.AddSingleton(_operationTypeStorage);
        services.AddSingleton<IPinHashService>(pinHashServiceMock);
        services.AddSingleton<AccountService>();

        ServiceProvider sp = services.BuildServiceProvider();

        _service = sp.GetRequiredService<AccountService>();
    }

    [Fact]
    public async Task WithdrawCorrect()
    {
        var accountId = Guid.NewGuid();
        var sessionKey = Guid.NewGuid();
        const decimal withdrawalAmount = 300m;
        const decimal initialBalance = 1000m;
        const decimal expectedNewBalance = 700m;

        var account = new Account
        {
            AccountId = accountId,
            Balance = initialBalance,
            PinHash = "hash",
        };

        _accountStorage.FindAccount(accountId).Returns(account);
        _operationTypeStorage.GetOperationTypeIdByName("WITHDRAW").Returns(2);

        OperationResult<decimal> result = await _service.Withdraw(accountId, withdrawalAmount, sessionKey);

        Assert.IsType<OperationResult<decimal>.Success>(result);
        var successResult = (OperationResult<decimal>.Success)result;
        Assert.Equal(expectedNewBalance, successResult.Result);

        await _accountStorage.Received(1)
            .UpdateAccount(
                Arg.Is<Account>(a =>
                    a.AccountId == accountId &&
                    a.Balance == expectedNewBalance));

        await _historyStorage.Received(1)
            .AddHistoryItem(
                Arg.Is<HistoryItem>(h =>
                    h.AccountId == accountId &&
                    h.OperationTypeId == 2 &&
                    h.Value == withdrawalAmount &&
                    h.Success == true));
    }

    [Fact]
    public async Task WithdrawWithoutMoney()
    {
        var accountId = Guid.NewGuid();
        var sessionKey = Guid.NewGuid();
        const decimal withdrawalAmount = 300m;
        const decimal initialBalance = 0m;

        var account = new Account
        {
            AccountId = accountId,
            Balance = initialBalance,
            PinHash = "hash",
        };

        _accountStorage.FindAccount(accountId).Returns(account);
        _operationTypeStorage.GetOperationTypeIdByName("WITHDRAW").Returns(2);

        OperationResult<decimal> result = await _service.Withdraw(accountId, withdrawalAmount, sessionKey);

        Assert.IsType<OperationResult<decimal>.BadRequest>(result);

        await _historyStorage.Received(1)
            .AddHistoryItem(
                Arg.Is<HistoryItem>(h =>
                    h.AccountId == accountId &&
                    h.OperationTypeId == 2 &&
                    h.Value == withdrawalAmount &&
                    h.Success == false));
    }

    [Fact]
    public async Task WithdrawWithLowBalance()
    {
        var accountId = Guid.NewGuid();
        var sessionKey = Guid.NewGuid();
        const decimal withdrawalAmount = 300m;
        const decimal initialBalance = 299m;

        var account = new Account
        {
            AccountId = accountId,
            Balance = initialBalance,
            PinHash = "hash",
        };

        _accountStorage.FindAccount(accountId).Returns(account);
        _operationTypeStorage.GetOperationTypeIdByName("WITHDRAW").Returns(2);

        OperationResult<decimal> result = await _service.Withdraw(accountId, withdrawalAmount, sessionKey);

        Assert.IsType<OperationResult<decimal>.BadRequest>(result);

        await _historyStorage.Received(1)
            .AddHistoryItem(
                Arg.Is<HistoryItem>(h =>
                    h.AccountId == accountId &&
                    h.OperationTypeId == 2 &&
                    h.Value == withdrawalAmount &&
                    h.Success == false));
    }

    [Fact]
    public async Task WithdrawCorrectCornerCase()
    {
        var accountId = Guid.NewGuid();
        var sessionKey = Guid.NewGuid();
        const decimal withdrawalAmount = 300m;
        const decimal initialBalance = 300m;
        const decimal expectedNewBalance = 0m;

        var account = new Account
        {
            AccountId = accountId,
            Balance = initialBalance,
            PinHash = "hash",
        };

        _accountStorage.FindAccount(accountId).Returns(account);
        _operationTypeStorage.GetOperationTypeIdByName("WITHDRAW").Returns(2);

        OperationResult<decimal> result = await _service.Withdraw(accountId, withdrawalAmount, sessionKey);

        Assert.IsType<OperationResult<decimal>.Success>(result);
        var successResult = (OperationResult<decimal>.Success)result;
        Assert.Equal(expectedNewBalance, successResult.Result);

        await _accountStorage.Received(1)
            .UpdateAccount(
                Arg.Is<Account>(a =>
                    a.AccountId == accountId &&
                    a.Balance == expectedNewBalance));

        await _historyStorage.Received(1)
            .AddHistoryItem(
                Arg.Is<HistoryItem>(h =>
                    h.AccountId == accountId &&
                    h.OperationTypeId == 2 &&
                    h.Value == withdrawalAmount &&
                    h.Success == true));
    }

    [Fact]
    public async Task DepositTest()
    {
        var accountId = Guid.NewGuid();
        var sessionKey = Guid.NewGuid();
        const decimal depositAmount = 300m;
        const decimal initialBalance = 300m;
        const decimal expectedNewBalance = 600m;

        var account = new Account
        {
            AccountId = accountId,
            Balance = initialBalance,
            PinHash = "hash",
        };

        _accountStorage.FindAccount(accountId).Returns(account);
        _operationTypeStorage.GetOperationTypeIdByName("DEPOSIT").Returns(3);

        OperationResult<decimal> result = await _service.Deposit(accountId, depositAmount, sessionKey);

        Assert.IsType<OperationResult<decimal>.Success>(result);

        var successResult = (OperationResult<decimal>.Success)result;
        Assert.Equal(expectedNewBalance, successResult.Result);

        await _accountStorage.Received(1)
            .UpdateAccount(
                Arg.Is<Account>(a =>
                    a.AccountId == accountId &&
                    a.Balance == expectedNewBalance));

        await _historyStorage.Received(1)
            .AddHistoryItem(
                Arg.Is<HistoryItem>(h =>
                    h.AccountId == accountId &&
                    h.OperationTypeId == 3 &&
                    h.Value == depositAmount &&
                    h.Success == true));
    }
}