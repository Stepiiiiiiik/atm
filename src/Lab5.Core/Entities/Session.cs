namespace Lab5.Core.Entities;

public class Session
{
    public Session() { }

    public Guid SessionKey { get; set; } = Guid.NewGuid();

    public int SessionTypeId { get; set; } = 0;

    public Guid? AccountId { get; set; }
}