using Lab5.Core.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Lab5.Core.Services;

public class PinHashService : IPinHashService
{
    public string GetHash(string pin)
    {
        return pin.Length != 4
            ? throw new ArgumentException("Pin must be 4 characters long")
            : Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(pin)));
    }

    public bool VerifyPin(string? pin, string? hash)
    {
        if (pin == null || hash == null)
            return false;

        byte[] actualHash = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
        byte[] expectedHash = Convert.FromBase64String(hash);

        return actualHash.SequenceEqual(expectedHash);
    }
}