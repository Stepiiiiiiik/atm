namespace Lab5.Core.Abstractions;

public interface ISessionService
{
    Task<OperationResult<Guid>> CreateAdminSession();

    Task<OperationResult<Guid>> CreateUserSession(Guid accountId);

    Task<OperationResult<bool>> IsAdmin(Guid sessionKey);

    Task<OperationResult<bool>> IsUser(Guid sessionKey);
}