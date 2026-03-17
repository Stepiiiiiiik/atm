namespace Lab5.Core.Entities;

public class Account
{
    public Account() { }

    public Account(string pinHash)
    {
        PinHash = pinHash;
    }

    public Guid AccountId { get; set; } = Guid.NewGuid();

    public string PinHash { get; set; } = string.Empty;

    public decimal Balance { get; set; } = 0;
}