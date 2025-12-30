using Galapa.Core.Utils;

namespace Galapa.Core.Tests.Utils;

public class Crc32Tests
{
    [Fact]
    public void Compute_ShouldReturnCorrectCrc32_ForKnownInput()
    {
        byte[] input = "emma\0"u8.ToArray();
        byte[] expected = [0xD6, 0x2E, 0x63, 0xF8];

        byte[] result = Crc32.Compute(input);

        Assert.Equal(expected, result);
    }
}