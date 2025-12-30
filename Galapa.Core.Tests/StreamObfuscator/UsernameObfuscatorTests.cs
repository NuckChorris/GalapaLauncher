using Galapa.Core.StreamObfuscator;

namespace Galapa.Core.Tests.StreamObfuscator;

public class UsernameObfuscatorTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenBaseStreamIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UsernameObfuscator(null!, "emma"));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUsernameIsNull()
    {
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => new UsernameObfuscator(stream, null!));
    }

    [Fact]
    public async void Read_ShouldXorDataWithUsernameKey()
    {
        byte[] input =
        [
            0xEA, 0x11, 0x1B, 0x95, 0xBA, 0x0E, 0x15, 0x9D, 0xA4, 0x5D, 0x0A, 0x97, 0xB8, 0x13, 0x41, 0xC9,
            0xF8, 0x1E, 0x41, 0xD8, 0xB3, 0x40, 0x63, 0x97, 0xB2, 0x47, 0x0D, 0x9F, 0xEB, 0x0C, 0x36, 0xAC,
            0x90, 0x03, 0x5B, 0xDA,
        ];
        var expected = "<?xml version=\"1.0\" encoding=\"UTF-8\""u8.ToArray();

        using var baseStream = new MemoryStream(input);
        await using var xorStream = new UsernameObfuscator(baseStream, "emma");

        var testStream = new MemoryStream();
        await xorStream.CopyToAsync(testStream);
        var result = testStream.ToArray();

        Assert.Equal(expected, result);
        Assert.Equal(input.Length, result.Length);
    }

    [Fact]
    public async void Write_ShouldXorDataWithUsernameKey()
    {
        byte[] expected =
        [
            0xEA, 0x11, 0x1B, 0x95, 0xBA, 0x0E, 0x15, 0x9D, 0xA4, 0x5D, 0x0A, 0x97, 0xB8, 0x13, 0x41, 0xC9,
            0xF8, 0x1E, 0x41, 0xD8, 0xB3, 0x40, 0x63, 0x97, 0xB2, 0x47, 0x0D, 0x9F, 0xEB, 0x0C, 0x36, 0xAC,
            0x90, 0x03, 0x5B, 0xDA,
        ];

        var input = "<?xml version=\"1.0\" encoding=\"UTF-8\""u8.ToArray();

        using var baseStream = new MemoryStream(input);
        await using var xorStream = new UsernameObfuscator(baseStream, "emma");

        var testStream = new MemoryStream();
        await xorStream.CopyToAsync(testStream);
        var result = testStream.ToArray();

        Assert.Equal(expected, result);
        Assert.Equal(input.Length, result.Length);
    }
}