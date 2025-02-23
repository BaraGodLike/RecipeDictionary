using System.Security.Cryptography;
using System.Text;

namespace RecipeDictionaryApi.Services;

public class PasswordHasher(
    int saltSize = PasswordHasher.DefaultSaltSize,
    int iterations = PasswordHasher.DefaultIterations)
{
    private const int DefaultSaltSize = 16;
    private const int DefaultIterations = 10000;
    private const int HashSize = 32;

    public byte[] HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password can't be empty.", nameof(password));

        var salt = GenerateSalt();
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var deriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations, HashAlgorithmName.SHA256);
        var hash = deriveBytes.GetBytes(HashSize);

        var finalHash = new byte[saltSize + HashSize];
        Buffer.BlockCopy(salt, 0, finalHash, 0, saltSize);
        Buffer.BlockCopy(hash, 0, finalHash, saltSize, HashSize);

        return finalHash;
    }

    public bool VerifyPassword(string password, byte[] storedHash)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password can't be empty.", nameof(password));
        if (storedHash == null || storedHash.Length != saltSize + HashSize)
            throw new ArgumentException("Incorrect format of cash", nameof(storedHash));

        var salt = new byte[saltSize];
        var hash = new byte[HashSize];
        Buffer.BlockCopy(storedHash, 0, salt, 0, saltSize);
        Buffer.BlockCopy(storedHash, saltSize, hash, 0, HashSize);

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var deriveBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations, HashAlgorithmName.SHA256);
        var computedHash = deriveBytes.GetBytes(HashSize);

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