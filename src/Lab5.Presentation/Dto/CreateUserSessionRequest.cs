namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;

public class CreateUserSessionRequest
{
    public string AccountId { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;
}