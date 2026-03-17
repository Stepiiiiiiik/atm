namespace Lab5.Core.Abstractions;

public interface ISessionTypeStorage
{
    Task<int?> GetSessionTypeIdByName(string sessionTypeName);
}