using Lab5.Core.Entities;

namespace Lab5.Core.Abstractions;

public interface IAccountStorage
{
    Task AddAccount(Account account);

    Task<Account?> FindAccount(Guid accountId);

    Task UpdateAccount(Account account);
}