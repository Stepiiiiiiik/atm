namespace Lab5.Core.Entities;

public class OperationType
{
    public OperationType() { }

    public OperationType(int operationTypeId, string operationTypeName)
    {
        OperationTypeId = operationTypeId;
        OperationTypeName = operationTypeName;
    }

    public int OperationTypeId { get; set; } = 0;

    public string OperationTypeName { get; set; } = string.Empty;
}