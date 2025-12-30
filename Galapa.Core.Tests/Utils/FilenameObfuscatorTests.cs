using Galapa.Core.Utils;

namespace Galapa.Core.Tests.Utils;

public class FilenameObfuscatorTests
{
    [Fact]
    public void Obfuscate_PlayerList()
    {
        string input = "dqxPlayerList.xml";
        int seed = 0x11;
        string expected = "cxjYxsgheGzie!iyx";

        string result = FilenameObfuscator.Obfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Obfuscate_KeyConfigFile()
    {
        string input = "KeyConfigFile.xml";
        int seed = 0x11;
        string expected = "FbrBrpgbkOhwj!ouc";

        string result = FilenameObfuscator.Obfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Obfuscate_PadButtonCaption()
    {
        string input = "PadButtonCaption.win32.xml";
        int seed = 0x1a;
        string expected = "NeaRxpzuwPlieoha!oub]#!xui";

        string result = FilenameObfuscator.Obfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Obfuscate_EnvironmentOption()
    {
        string input = "EnvironmentOption.win32.xml";
        int seed = 0x1b;
        string expected = "YzjadjylzqvBmsvub!zni]]!vnh";

        string result = FilenameObfuscator.Obfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Deobfuscate_PlayerList()
    {
        string input = "cxjYxsgheGzie!iyx";
        int seed = 0x11;
        string expected = "dqxPlayerList.xml";

        string result = FilenameObfuscator.Deobfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Deobfuscate_KeyConfigFile()
    {
        string input = "FbrBrpgbkOhwj!ouc";
        int seed = 0x11;
        string expected = "KeyConfigFile.xml";

        string result = FilenameObfuscator.Deobfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Deobfuscate_PadButtonCaption()
    {
        string input = "NeaRxpzuwPlieoha!oub]#!xui";
        int seed = 0x1a;
        string expected = "PadButtonCaption.win32.xml";

        string result = FilenameObfuscator.Deobfuscate(input, seed);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Deobfuscate_EnvironmentOption()
    {
        string input = "YzjadjylzqvBmsvub!zni]]!vnh";
        int seed = 0x1b;
        string expected = "EnvironmentOption.win32.xml";

        string result = FilenameObfuscator.Deobfuscate(input, seed);

        Assert.Equal(expected, result);
    }
}