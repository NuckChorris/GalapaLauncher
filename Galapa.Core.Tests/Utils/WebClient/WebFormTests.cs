using Galapa.Core.Web;
using HtmlAgilityPack;

namespace Galapa.Core.Tests.Utils.WebClient;

public class WebFormTests
{
    [Fact]
    public void FromHtmlForm_ShouldParseFormCorrectly()
    {
        // Arrange
        var html = @"
            <form action='/submit' method='post'>
                <input type='text' name='field1' value='value1' />
                <input type='hidden' name='field2' value='value2' />
            </form>";
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var formNode = doc.DocumentNode.SelectSingleNode("//form");

        // Act
        var form = WebForm.FromHtmlForm(formNode, "https://example.com");

        // Assert
        Assert.Equal(HttpMethod.Post, form.Method);
        Assert.Equal("https://example.com/submit", form.Action);
        Assert.Equal(2, form.Fields.Count);
        Assert.Equal("value1", form.Fields["field1"]);
        Assert.Equal("value2", form.Fields["field2"]);
    }

    [Fact]
    public void FromHtmlForm_ShouldHandleEmptyAction()
    {
        // Arrange
        var html = @"
            <form method='get'>
                <input type='text' name='field1' value='value1' />
            </form>";
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var formNode = doc.DocumentNode.SelectSingleNode("//form");

        // Act
        var form = WebForm.FromHtmlForm(formNode, "https://example.com");

        // Assert
        Assert.Equal(HttpMethod.Get, form.Method);
        Assert.Equal("https://example.com/", form.Action);
        Assert.Single(form.Fields);
        Assert.Equal("value1", form.Fields["field1"]);
    }
}