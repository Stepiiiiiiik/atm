namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;

public class AccountDepositRequest
{
    public string SessionKey { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}