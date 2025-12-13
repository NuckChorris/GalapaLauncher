using System.Text.RegularExpressions;
using DQXLauncher.Core.Utils.WebClient;
using HtmlAgilityPack;

namespace DQXLauncher.Core.Hiroba;

public partial class BannerImage
{
    public string Alt { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Src { get; set; } = string.Empty;

    [GeneratedRegex(@"javascript:ctrLinkAction\('link=([^']+)'\);")]
    private static partial Regex HrefFormat();

    public static async Task<List<BannerImage>> GetBanners()
    {
        var httpClient = await WebClient.Get("banners");

        var doc = new HtmlDocument();
        var result = await httpClient.GetAsync("https://hiroba.dqx.jp/sc/rotationbanner");
        doc.LoadHtml(await result.Content.ReadAsStringAsync());

        return doc.DocumentNode.SelectNodes("//li[contains(@class, 'slide')]/a").Select(slide =>
        {
            var img = slide.SelectSingleNode(".//img");
            var match = HrefFormat().Match(slide.GetAttributeValue("href", string.Empty));
            var href = match.Success ? match.Groups[1].Value : string.Empty;
            //"javascript:ctrLinkAction('link=https://hiroba.dqx.jp/sc/topics/detail/39539f630a3b94d3ed61ea9d04c9bb05/');"


            return new BannerImage
            {
                Alt = slide.GetAttributeValue("alt", string.Empty),
                Href = href,
                Src = img.GetAttributeValue("src", string.Empty)
            };
        }).ToList();
    }
}