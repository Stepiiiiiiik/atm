namespace Lab5.Core.Abstractions;

public interface IOperationTypeStorage
{
    Task<int?> GetOperationTypeIdByName(string operationName);
}