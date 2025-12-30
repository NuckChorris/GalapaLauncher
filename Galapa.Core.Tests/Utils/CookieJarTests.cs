using System.Net;
using Galapa.Core.Configuration;
using Galapa.Core.Utils;
using Galapa.TestUtilities;

namespace Galapa.Core.Tests.Utils;

[Collection("Sequential")]
public class CookieJarTests
{
    // FakeHandler captures the request and returns a response with a Set-Cookie header.
    private class FakeHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            // Only add Set-Cookie header if not already present (simulate first call).
            if (!request.Headers.Contains("Cookie"))
                response.Headers.Add("Set-Cookie",
                    "TestCookie=TestValue; Path=/; Expires=Wed, 21 Oct 2099 07:28:00 GMT");

            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task TestCookieJar_SavesCookieFromResponse()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        var fakeHandler = new FakeHandler();
        var cookieJar = await CookieJar.HandlerForJar(fakeHandler, "TestJar_Save");
        var client = new HttpClient(cookieJar);

        // Send first request to trigger saving a cookie.
        await client.GetAsync("http://example.com/testpath");
        // Send second request; cookie should now be added.

        fakeHandler = new FakeHandler();
        // Reuse the same cookieJar with a new inner handler to capture the next request.
        cookieJar = await CookieJar.HandlerForJar(fakeHandler, "TestJar_Save");
        client = new HttpClient(cookieJar);
        // This request should now include the cookie.
        await client.GetAsync("http://example.com/testpath");

        Assert.NotNull(fakeHandler.LastRequest);
        // Check that a Cookie header is attached.
        Assert.True(fakeHandler.LastRequest.Headers.Contains("Cookie"));
    }

    [Fact]
    public async Task TestCookieJar_SendsCookieHeaderOnSubsequentRequests()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;
        var fakeHandler = new FakeHandler();
        var cookieJar = await CookieJar.HandlerForJar(fakeHandler, "TestJar_Send");
        var client = new HttpClient(cookieJar);
        // First request to get cookie from Set-Cookie header.
        await client.GetAsync("http://example.com/test");
        // Use a new FakeHandler to capture outgoing request headers.
        var captureHandler = new FakeHandler();
        cookieJar = await CookieJar.HandlerForJar(captureHandler, "TestJar_Send");
        client = new HttpClient(cookieJar);
        await client.GetAsync("http://example.com/test");
        Assert.NotNull(captureHandler.LastRequest);
        // Verify that the Cookie header contains our test cookie.
        var cookieHeader = captureHandler.LastRequest.Headers.GetValues("Cookie");
        Assert.Contains("TestCookie=TestValue", string.Join(";", cookieHeader));
    }

    [Fact]
    public async Task TestCookieJar_DeletesExpiredCookies()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        var fakeHandler = new FakeHandler();
        var cookieJar = await CookieJar.HandlerForJar(fakeHandler, "TestJar_Expired");
        var client = new HttpClient(cookieJar);

        // Bake some cookies
        var expiredCookie = new Cookie("ExpiredCookie", "ExpiredValue", "/", "example.com")
            { Expires = DateTime.UtcNow.AddDays(-1) }; // Expired yesterday
        var validCookie = new Cookie("ValidCookie", "ValidValue", "/", "example.com")
            { Expires = DateTime.UtcNow.AddDays(1) }; // Expires tomorrow
        cookieJar.Cookies.GetOrCreate("example.com").Add(expiredCookie);
        cookieJar.Cookies.GetOrCreate("example.com").Add(validCookie);

        await client.GetAsync("http://example.com/test");
        Assert.NotNull(fakeHandler.LastRequest);
        // Verify that the Cookie header contains our test cookie.
        var cookieHeader = fakeHandler.LastRequest.Headers.GetValues("Cookie");
        var enumerable = cookieHeader as string[] ?? cookieHeader.ToArray();
        Assert.Contains("ValidCookie=ValidValue", string.Join(";", enumerable));
        Assert.DoesNotContain("ExpiredCookie=ExpiredValue", string.Join(";", enumerable));
    }

    [Fact]
    public async Task TestCookieJar_ClearRemovesAllCookies()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        var fakeHandler = new FakeHandler();
        var cookieJar = await CookieJar.HandlerForJar(fakeHandler, "TestJar_Clear");

        // Add some cookies
        var cookie1 = new Cookie("Cookie1", "Value1", "/", "example.com")
            { Expires = DateTime.UtcNow.AddDays(1) };
        var cookie2 = new Cookie("Cookie2", "Value2", "/", "example.com")
            { Expires = DateTime.UtcNow.AddDays(1) };
        cookieJar.Cookies.GetOrCreate("example.com").Add(cookie1);
        cookieJar.Cookies.GetOrCreate("example.com").Add(cookie2);

        // Verify cookies are present
        Assert.NotEmpty(cookieJar.Cookies["example.com"]);

        // Clear cookies
        await cookieJar.Clear();

        // Verify cookies are removed
        Assert.False(cookieJar.Cookies.ContainsKey("example.com"));
    }
}