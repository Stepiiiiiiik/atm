namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;

public class AccountWithdrawRequest
{
    public string SessionKey { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}