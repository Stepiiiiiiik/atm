namespace Lab5.Core.Abstractions;

public interface ILoginService
{
    Task<OperationResult<bool>> LoginAdmin(Guid sessionId);

    Task<OperationResult<Guid>> LoginUser(Guid sessionId);
}