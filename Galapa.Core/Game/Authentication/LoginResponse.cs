using Galapa.Core.Web;
using HtmlAgilityPack;

namespace Galapa.Core.Game.Authentication;

public record LoginResponse
{
    public required HttpResponseMessage Response { get; init; }
    public required HtmlDocument Document { get; init; }

    // ReSharper disable once ReturnTypeCanBeNotNullable
    public HtmlNode? SqexAuth => this.Document.DocumentNode.SelectSingleNode("//x-sqexauth");

    public WebForm? Form
    {
        get
        {
            var form = this.Document.DocumentNode.SelectSingleNode("//form[@name='mainForm']");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (form is null) return null;
            return WebForm.FromHtmlForm(form, this.Response.RequestMessage!.RequestUri!);
        }
    }

    public static async Task<LoginResponse> FromHttpResponse(HttpResponseMessage response)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        return new LoginResponse
        {
            Document = doc,
            Response = response
        };
    }

    // HtmlAgilityPack lies about the nullability of its properties, these are *very* nullable.
    // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public string? ErrorMessage => this.SqexAuth?.Attributes["message"]?.Value;
    public string? SessionId => this.SqexAuth?.Attributes["sid"]?.Value;
    public string? Lang => this.SqexAuth?.Attributes["lang"]?.Value;
    public string? Region => this.SqexAuth?.Attributes["region"]?.Value;
    public string? Utc => this.SqexAuth?.Attributes["utc"]?.Value;
    public string? Mode => this.SqexAuth?.Attributes["mode"]?.Value;

    public string? Token => this.SqexAuth?.Attributes["id"]?.Value;
    // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
}