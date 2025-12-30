using Galapa.Core.StreamObfuscator;

namespace Galapa.Core.Tests.StreamObfuscator;

public class FixedObfuscatorTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenBaseStreamIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FixedObfuscator(null!));
    }

    [Fact]
    public async void Read_ShouldXorDataWithA7()
    {
        byte[] input =
        [
            0x9B, 0x98, 0xDF, 0xCA, 0xCB, 0x87, 0xD1, 0xC2, 0xD5, 0xD4, 0xCE, 0xC8, 0xC9, 0x9A, 0x85, 0x96,
            0x89, 0x97, 0x85, 0x87, 0xC2, 0xC9, 0xC4, 0xC8, 0xC3, 0xCE, 0xC9, 0xC0, 0x9A, 0x85, 0xF2, 0xF3,
            0xE1, 0x8A, 0x9F, 0x85
        ];
        var expected = "<?xml version=\"1.0\" encoding=\"UTF-8\""u8.ToArray();

        using var baseStream = new MemoryStream(input);
        await using var xorStream = new FixedObfuscator(baseStream);

        var testStream = new MemoryStream();
        await xorStream.CopyToAsync(testStream);
        var result = testStream.ToArray();

        Assert.Equal(expected, result);
        Assert.Equal(input.Length, result.Length);
    }

    [Fact]
    public async void Write_ShouldXorDataWithA7()
    {
        var input = "<?xml version=\"1.0\" encoding=\"UTF-8\""u8.ToArray();
        byte[] expected =
        [
            0x9B, 0x98, 0xDF, 0xCA, 0xCB, 0x87, 0xD1, 0xC2, 0xD5, 0xD4, 0xCE, 0xC8, 0xC9, 0x9A, 0x85, 0x96,
            0x89, 0x97, 0x85, 0x87, 0xC2, 0xC9, 0xC4, 0xC8, 0xC3, 0xCE, 0xC9, 0xC0, 0x9A, 0x85, 0xF2, 0xF3,
            0xE1, 0x8A, 0x9F, 0x85
        ];

        using var baseStream = new MemoryStream(input);
        await using var xorStream = new FixedObfuscator(baseStream);

        var testStream = new MemoryStream();
        await xorStream.CopyToAsync(testStream);
        var result = testStream.ToArray();

        Assert.Equal(expected, result);
        Assert.Equal(input.Length, result.Length);
    }
}