using Lab5.Core.Entities;

namespace Lab5.Core.Abstractions;

public interface IHistoryStorage
{
    Task AddHistoryItem(HistoryItem historyItem);

    Task<IReadOnlyCollection<HistoryItem>> GetHistoryByAccountId(Guid accountId);
}