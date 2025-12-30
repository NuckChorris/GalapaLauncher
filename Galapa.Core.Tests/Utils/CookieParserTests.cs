using System.Globalization;
using Galapa.Core.Utils;

namespace Galapa.Core.Tests.Utils;

public class CookieParserTests
{
    [Fact]
    public void ParseCookie_WithExpires_ShouldParseCorrectly()
    {
        // Arrange
        var header = "DQXLogin=1234567890; Path=/; Secure; Expires=Sat, 21 Oct 2023 07:28:00 GMT";
        var domain = "example.com";
        var expected = DateTime.ParseExact("Sat, 21 Oct 2023 07:28:00 GMT", "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
            CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        // Act
        var cookie = CookieParser.ParseCookie(header, domain);

        // Assert
        Assert.Equal("DQXLogin", cookie.Name);
        Assert.Equal("1234567890", cookie.Value);
        Assert.Equal("/", cookie.Path);
        Assert.Equal("example.com", cookie.Domain);
        Assert.True(cookie.Secure);
        Assert.False(cookie.HttpOnly);
        Assert.Equal(expected, cookie.Expires);
    }

    [Fact]
    public void ParseCookie_WithMaxAge_ShouldSetExpiresRelativeToNow()
    {
        // Arrange
        var header = "TestCookie=abc; Max-Age=3600; Path=/";
        var domain = "test.com";
        var before = DateTime.UtcNow;

        // Act
        var cookie = CookieParser.ParseCookie(header, domain);
        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal("TestCookie", cookie.Name);
        Assert.Equal("abc", cookie.Value);
        Assert.Equal("/", cookie.Path);
        Assert.Equal("test.com", cookie.Domain);
        // Allow a tolerance of 2 seconds between parsing and expectation.
        var expectedLowerBound = before.AddSeconds(3600);
        var expectedUpperBound = after.AddSeconds(3600);
        Assert.InRange(cookie.Expires, expectedLowerBound, expectedUpperBound);
    }

    [Fact]
    public void ParseCookie_WithEmptyExpires_ShouldDefaultToMinValue()
    {
        // Arrange
        var header = "MissingDate=val; Expires=; Path=/";
        var domain = "empty.com";
        // Act
        var cookie = CookieParser.ParseCookie(header, domain);

        // Assert
        Assert.Equal("MissingDate", cookie.Name);
        Assert.Equal("val", cookie.Value);
        Assert.Equal("/", cookie.Path);
        Assert.Equal("empty.com", cookie.Domain);
        // Default Expires should be set to DateTime.MinValue per our logic
        Assert.Equal(DateTime.MinValue, cookie.Expires);
    }

    [Fact]
    public void ParseCookie_WithInvalidExpires_ShouldDefaultToMinValue()
    {
        // Arrange
        var header = "MissingDate=val; Expires=today; Path=/";
        var domain = "empty.com";
        // Act
        var cookie = CookieParser.ParseCookie(header, domain);

        // Assert
        Assert.Equal("MissingDate", cookie.Name);
        Assert.Equal("val", cookie.Value);
        Assert.Equal("/", cookie.Path);
        Assert.Equal("empty.com", cookie.Domain);
        // Default Expires should be set to DateTime.MinValue per our logic
        Assert.Equal(DateTime.MinValue, cookie.Expires);
    }
}