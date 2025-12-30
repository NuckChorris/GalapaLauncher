namespace Galapa.Core.StreamObfuscator;

/// <summary>
///     This obfuscator applies a fixed 1-byte XOR key of 0xA7 to the stream.
/// </summary>
/// <remarks>
///     This is used for a lot of common config files, but not all. See the
///     <see cref="UsernameObfuscator">UsernameObfuscator</see> for another common type of obfuscator which uses a dynamic
///     key derived from the system username.
/// </remarks>
/// <param name="baseStream">The stream to process</param>
public class FixedObfuscator(Stream baseStream) : XorObfuscator(baseStream, [0xA7])
{
    public static Func<Stream, Stream> Factory => s => new FixedObfuscator(s);
}