using Lab5.Core.Entities;

namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;

public class HistoryItemDto
{
    public int OperationTypeId { get; set; } = 0;

    public decimal? Value { get; set; }

    public bool Success { get; set; }

    public DateTime CreatedAt { get; set; }

    public static HistoryItemDto CreateFrom(HistoryItem historyItem)
    {
        return new HistoryItemDto
        {
            OperationTypeId = historyItem.OperationTypeId,
            Value = historyItem.Value,
            Success = historyItem.Success,
            CreatedAt = historyItem.CreatedAt,
        };
    }
}