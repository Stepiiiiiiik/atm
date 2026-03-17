using Lab5.Core.Abstractions;
using Lab5.Core.Entities;

namespace Lab5.Core.Services;

public class LoginService : ILoginService
{
    private readonly ISessionStorage _sessionStorage;

    private readonly ISessionTypeStorage _sessionTypeStorage;

    public LoginService(ISessionStorage sessionStorage, ISessionTypeStorage sessionTypeStorage)
    {
        _sessionStorage = sessionStorage;
        _sessionTypeStorage = sessionTypeStorage;
    }

    public async Task<OperationResult<bool>> LoginAdmin(Guid sessionId)
    {
        int? sessionType = await _sessionTypeStorage.GetSessionTypeIdByName("ADMIN");
        if (sessionType == null)
        {
            return new OperationResult<bool>.InternalError("Invalid session type");
        }

        Session? session = await _sessionStorage.FindSession(sessionId);

        return session == null
            ? new OperationResult<bool>.BadRequest("Invalid session key")
            : session.SessionTypeId == sessionType.Value
                ? new OperationResult<bool>.Success(true)
                : new OperationResult<bool>.BadRequest("Access denied. It is not admin session");
    }

    public async Task<OperationResult<Guid>> LoginUser(Guid sessionId)
    {
        int? sessionType = await _sessionTypeStorage.GetSessionTypeIdByName("USER");
        if (sessionType == null)
        {
            return new OperationResult<Guid>.InternalError("Invalid session type");
        }

        Session? session = await _sessionStorage.FindSession(sessionId);

        return session == null
            ? new OperationResult<Guid>.BadRequest("Invalid session key")
            : session.AccountId == null
                ? new OperationResult<Guid>.BadRequest("Invalid session")
                : session.SessionTypeId == sessionType.Value
                    ? new OperationResult<Guid>.Success(session.AccountId ?? Guid.NewGuid())
                    : new OperationResult<Guid>.BadRequest("Access denied. It is not user session");
    }
}