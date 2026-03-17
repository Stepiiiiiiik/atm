using Lab5.Core.Entities;

namespace Lab5.Core.Abstractions;

public interface ISessionStorage
{
    Task<Session?> FindSession(Guid sessionKey);

    Task<OperationResult<Guid>> AddSession(Session session);
}