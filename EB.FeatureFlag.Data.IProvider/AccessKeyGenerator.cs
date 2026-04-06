using System.Security.Cryptography;

namespace EB.FeatureFlag.Data.IProvider;

public static class AccessKeyGenerator
{
    private const int KeySizeInBytes = 32;

    /// <summary>
    /// Generates a cryptographically secure access key (Base64-encoded, 32 bytes).
    /// </summary>
    public static string GenerateAccessKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(KeySizeInBytes);
        return Convert.ToBase64String(bytes);
    }
}
