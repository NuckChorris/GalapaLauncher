using System.Net;
using System.Text;
using Galapa.Core.Game.LoginStrategy;

namespace Galapa.Core.Tests.Game.LoginStrategy;

public class LoginResponseTests
{
    private string CreateMockSidHtml(string? sid)
    {
        return @$"
            <html>
                <body>
                    <x-sqexauth sid=""{sid}"" lang=""ja-jp"" region=""1"" utc=""1743442069"" mode=""1"" />
                </body>
            </html>
        ";
    }

    private string CreateMockMessageHtml(string? message)
    {
        return @$"
            <html>
                <body>
                    <x-sqexauth message=""{message}"" />
                    <form action=""login?client_id=dqx_win&amp;redirect_uri=https%3A%2F%2Fdqx-login.square-enix.com%2F&amp;response_type=code&amp;lv=2&amp;alar=1"" method=""post"" name=""mainForm"">
                        <input name=""sqexid"" id=""sqexid"" type=""text""/>
                        <input name=""password"" id=""passwd"" type=""password""/>
                        <input type=""hidden"" name=""_STORED_"" value=""stored_value"">
                    </form>
                </body>
            </html>
        ";
    }

    private HttpResponseMessage CreateMockResponse(string html)
    {
        var request =
            new HttpRequestMessage(HttpMethod.Post, "http://dqx-login.square-enix.com/oauth/sp/sso/dqxwin/login");
        var content = new StringContent(html, Encoding.UTF8, "text/html");
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = content, RequestMessage = request };
    }

    [Fact]
    public async Task FromResponse_ParsesMessageCorrectly()
    {
        var html = CreateMockMessageHtml("Login failed");
        var response = CreateMockResponse(html);

        var loginResponse = await LoginResponse.FromHttpResponse(response);

        Assert.Equal("Login failed", loginResponse.ErrorMessage);
        Assert.Null(loginResponse.SessionId);
        Assert.Null(loginResponse.Lang);
        Assert.Null(loginResponse.Region);
        Assert.Null(loginResponse.Utc);
        Assert.Null(loginResponse.Mode);
        Assert.NotNull(loginResponse.SqexAuth);
    }

    [Fact]
    public async Task FromResponse_ParsesSidCorrectly()
    {
        var html = CreateMockSidHtml("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
        var response = CreateMockResponse(html);

        var loginResponse = await LoginResponse.FromHttpResponse(response);

        Assert.Null(loginResponse.ErrorMessage);
        Assert.Equal("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef", loginResponse.SessionId);
        Assert.Equal("ja-jp", loginResponse.Lang);
        Assert.Equal("1", loginResponse.Region);
        Assert.Equal("1743442069", loginResponse.Utc);
        Assert.Equal("1", loginResponse.Mode);
        Assert.NotNull(loginResponse.SqexAuth);
    }

    [Fact]
    public async Task Form_ReturnsNullWhenMissing()
    {
        var html = CreateMockSidHtml("1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef");
        var response = CreateMockResponse(html);

        var loginResponse = await LoginResponse.FromHttpResponse(response);

        Assert.Null(loginResponse.Form);
    }

    [Fact]
    public async Task SqexAuth_ReturnsNullWhenMissing()
    {
        var html = "<html><body></body></html>";
        var response = CreateMockResponse(html);

        var loginResponse = await LoginResponse.FromHttpResponse(response);

        Assert.Null(loginResponse.SqexAuth);
    }

    [Fact]
    public async Task Utc_ReturnsNullWhenSqexAuthMissing()
    {
        var html = "<html><body></body></html>";
        var response = CreateMockResponse(html);

        var loginResponse = await LoginResponse.FromHttpResponse(response);

        Assert.Null(loginResponse.Utc);
    }
}