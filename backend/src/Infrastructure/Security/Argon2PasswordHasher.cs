using Application.Abstractions.Security;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public sealed class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemorySize = 65536;
    private const int DegreeOfParallelism = 2;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashInternal(password, salt);
        return $"argon2id${Iterations}${MemorySize}${DegreeOfParallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 6 || !parts[0].Equals("argon2id", StringComparison.OrdinalIgnoreCase)) return false;

        if (!int.TryParse(parts[1], out var iterations) ||
            !int.TryParse(parts[2], out var memorySize) ||
            !int.TryParse(parts[3], out var parallelism))
            return false;

        var salt = Convert.FromBase64String(parts[4]);
        var expectedHash = Convert.FromBase64String(parts[5]);
        var actualHash = HashInternal(password, salt, iterations, memorySize, parallelism);
        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }

    private static byte[] HashInternal(string password, byte[] salt, int iterations = Iterations, int memorySize = MemorySize, int parallelism = DegreeOfParallelism)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = parallelism,
            MemorySize = memorySize,
            Iterations = iterations
        };

        return argon2.GetBytes(HashSize);
    }
}
