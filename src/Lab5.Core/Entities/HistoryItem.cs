namespace Lab5.Core.Entities;

public class HistoryItem
{
    public HistoryItem() { }

    public Guid HistoryItemId { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; } = Guid.NewGuid();

    public int OperationTypeId { get; set; } = 0;

    public decimal? Value { get; set; }

    public Guid SessionKey { get; set; } = Guid.NewGuid();

    public bool Success { get; set; } = false;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}