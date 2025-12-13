using System.Text;
using DQXLauncher.Core.Utils;

namespace DQXLauncher.Core.StreamObfuscator;

/// <summary>
///     This obfuscator applies a repeating 4-byte XOR key based on the provided username.
/// </summary>
/// <remarks>
///     This seems to be used for more "sensitive" config files, such as the player list. See the
///     <see cref="FixedObfuscator">FixedObfuscator</see> for the other common type of obfuscator.
/// </remarks>
/// <param name="baseStream">The stream to process</param>
/// <param name="username">The username to derive the key from</param>
public class UsernameObfuscator(Stream baseStream, string username)
    : XorObfuscator(baseStream, GenerateXorKeyFromUsername(username))
{
    public static Func<Stream, Stream> Factory => s => new UsernameObfuscator(s, Environment.UserName);

    // Player lists are XORed with a key derived from the username (it's just crc32)
    private static byte[] GenerateXorKeyFromUsername(string username)
    {
        if (username is null) throw new ArgumentNullException(nameof(username));
        return Crc32.Compute(Encoding.ASCII.GetBytes($"{username}\0"));
    }
}