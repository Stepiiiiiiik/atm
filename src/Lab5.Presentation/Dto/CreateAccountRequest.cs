namespace Itmo.ObjectOrientedProgramming.Lab5.Presentation.Dto;

public class CreateAccountRequest
{
    public string SessionKey { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;
}