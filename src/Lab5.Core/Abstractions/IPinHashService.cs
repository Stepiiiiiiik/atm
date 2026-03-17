namespace Lab5.Core.Abstractions;

public interface IPinHashService
{
    string GetHash(string pin);

    bool VerifyPin(string pin, string hash);
}