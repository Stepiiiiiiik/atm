using Lab5.Core.Abstractions;
using Lab5.Core.Entities;

namespace Lab5.Core.Services;

public class SessionService : ISessionService
{
    private readonly ISessionStorage _sessionStorage;

    private readonly ISessionTypeStorage _sessionTypeStorage;

    public SessionService(ISessionStorage sessionStorage, ISessionTypeStorage sessionTypeStorage)
    {
        _sessionStorage = sessionStorage;
        _sessionTypeStorage = sessionTypeStorage;
    }

    public async Task<OperationResult<Guid>> CreateAdminSession()
    {
        int? sessionType = await _sessionTypeStorage.GetSessionTypeIdByName("ADMIN");
        if (sessionType == null)
        {
            return new OperationResult<Guid>.InternalError("Invalid session type");
        }

        var session = new Session
        {
            SessionTypeId = sessionType.Value,
            AccountId = null,
        };

        await _sessionStorage.AddSession(session);

        return new OperationResult<Guid>.Success(session.SessionKey);
    }

    public async Task<OperationResult<Guid>> CreateUserSession(Guid accountId)
    {
        int? sessionType = await _sessionTypeStorage.GetSessionTypeIdByName("USER");
        if (sessionType == null)
        {
            return new OperationResult<Guid>.InternalError("Invalid session type");
        }

        var session = new Session
        {
            SessionTypeId = sessionType.Value,
            AccountId = accountId,
        };

        await _sessionStorage.AddSession(session);

        return new OperationResult<Guid>.Success(session.SessionKey);
    }

    public async Task<OperationResult<bool>> IsAdmin(Guid sessionKey)
    {
        int? sessionType = await _sessionTypeStorage.GetSessionTypeIdByName("ADMIN");
        if (sessionType == null)
        {
            return new OperationResult<bool>.InternalError("Invalid session type");
        }

        Session? session = await _sessionStorage.FindSession(sessionKey);
        return session == null
            ? new OperationResult<bool>.InternalError("Invalid session key")
            : new OperationResult<bool>.Success(session.SessionTypeId == sessionType.Value);
    }

    public async Task<OperationResult<bool>> IsUser(Guid sessionKey)
    {
        int? sessionType = await _sessionTypeStorage.GetSessionTypeIdByName("USER");
        if (sessionType == null)
        {
            return new OperationResult<bool>.InternalError("Invalid session type");
        }

        Session? session = await _sessionStorage.FindSession(sessionKey);
        return session == null
            ? new OperationResult<bool>.InternalError("Invalid session key")
            : new OperationResult<bool>.Success(session.SessionTypeId == sessionType.Value);
    }
}