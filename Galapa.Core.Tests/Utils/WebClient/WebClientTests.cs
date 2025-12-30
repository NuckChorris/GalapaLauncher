using System.Net;
using Galapa.Core.Configuration;
using Galapa.Core.Utils;
using Galapa.TestUtilities;
using Moq;
using Moq.Protected;
// We have a namespace conflict lol
using WebClientNS = Galapa.Core.Utils.WebClient;

namespace Galapa.Core.Tests.Utils.WebClient;

public class WebClientTests
{
    [Fact]
    public async Task Get_ShouldReturnSameInstanceForSameKey()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        var client1 = await WebClientNS.WebClient.Get("testKey");
        var client2 = await WebClientNS.WebClient.Get("testKey");

        Assert.Same(client1, client2);
    }

    [Fact]
    public async Task SendFormAsync_ShouldSendFormCorrectly()
    {
        using var tempDir = new TempDirectory();
        Paths.AppData = tempDir.Path;

        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Success")
            });

        var handler = await CookieJar.HandlerForJar(mockHandler.Object, "testKey");
        var client = new WebClientNS.WebClient(handler);

        var form = new WebClientNS.WebForm
        {
            Method = HttpMethod.Post,
            Action = "https://example.com/submit",
            Fields = new Dictionary<string, string>
            {
                { "field1", "value1" },
                { "field2", "value2" }
            }
        };

        // Act
        var response = await client.SendFormAsync(form);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri == new Uri("https://example.com/submit") &&
                req.Content is FormUrlEncodedContent),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}