using System.Security.Cryptography;
using System.Text;

namespace RecipeDictionaryApi.Services;

public class PasswordHasher(byte[] key, int saltSize = 16, int iterations = 10000)
{
    private readonly byte[] _key = key;

    public byte[] HashPassword(string password)
    {
        var salt = GenerateSalt();
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var deriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations, HashAlgorithmName.SHA256);
        var hash = deriveBytes.GetBytes(saltSize);

        var finalHash = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, finalHash, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, finalHash, salt.Length, hash.Length);

        return finalHash;
    }

    public bool VerifyPassword(string password, byte[] storedHash)
    {
        var salt = new byte[saltSize];
        var hash = new byte[storedHash.Length - saltSize];

        Buffer.BlockCopy(storedHash, 0, salt, 0, salt.Length);
        Buffer.BlockCopy(storedHash, salt.Length, hash, 0, hash.Length);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var deriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations, HashAlgorithmName.SHA256);
        var computedHash = deriveBytes.GetBytes(hash.Length);

        return CryptographicOperations.FixedTimeEquals(computedHash, hash);
    }

    private byte[] GenerateSalt()
    {
        var salt = new byte[saltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
}
