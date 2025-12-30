using System.Net;
using HtmlAgilityPack;

namespace Galapa.Core.Utils.WebClient;

public class WebForm
{
    public required HttpMethod Method { get; set; }
    public required string Action { get; set; }
    public required Dictionary<string, string> Fields { get; set; }

    public static WebForm FromHtmlForm(HtmlNode form, string baseUrl)
    {
        return FromHtmlForm(form, new Uri(baseUrl));
    }

    public static WebForm FromHtmlForm(HtmlNode form, Uri baseUrl)
    {
        var action = new Uri(
            baseUrl,
            WebUtility.HtmlDecode(form.GetAttributeValue("action", ""))
        );

        return new WebForm
        {
            Method = GetMethod(form.GetAttributeValue("method", string.Empty)),
            Action = action.ToString(),
            Fields = form
                .Descendants("input")
                .Where(node => node.GetAttributeValue("name", "") != "") // Only include inputs with a name
                .ToDictionary(
                    node => node.GetAttributeValue("name", ""),
                    node => WebUtility.HtmlDecode(node.GetAttributeValue("value", ""))
                )
        };
    }

    private static HttpMethod GetMethod(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            "PATCH" => HttpMethod.Patch,
            _ => new HttpMethod(method.ToUpperInvariant())
        };
    }
}