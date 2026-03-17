namespace Lab5.Core.Entities;

public class SessionType
{
    public SessionType() { }

    public SessionType(int sessionTypeId, string sessionTypeName)
    {
        SessionTypeId = sessionTypeId;
        SessionTypeName = sessionTypeName;
    }

    public int SessionTypeId { get; set; } = 0;

    public string SessionTypeName { get; set; } = string.Empty;
}