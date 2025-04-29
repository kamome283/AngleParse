using System.Linq;
using System.Threading.Tasks;
using AngleParse.Resource;
using AngleParse.Selector;
using Xunit;
using static AngleParse.Test.Helpers.ElementResourceFactory;

namespace AngleParse.Test.Selector;

public class AttributeSelectorTests
{
    [Fact]
    public void Create()
    {
        const Attr attribute = Attr.Href;
        var attributeSelector = SelectorFactory.CreateSelector(attribute);
        Assert.IsType<AttributeSelector>(attributeSelector);
    }

    [Fact]
    public async Task SelectInnerHtml()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.InnerHtml);
        var resource = await CreateElementResourceAsync("no-attribute.html");
        var result = selector.Select(resource).Select(r => r.String);
        const string expected = "<span>some link</span>";
        Assert.Single(result, item => item == expected);
    }

    [Fact]
    public async Task SelectOuterHtml()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.OuterHtml);
        var resource = await CreateElementResourceAsync("no-attribute.html");
        var result = selector.Select(resource).Select(r => r.String);
        const string expected = "<a><span>some link</span></a>";
        Assert.Single(result, item => item == expected);
    }

    [Fact]
    public async Task SelectTextContent()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.TextContent);
        var resource = await CreateElementResourceAsync("no-attribute.html");
        var result = selector.Select(resource).Select(r => r.String).ToList();
        const string expected = "some link";
        Assert.Single(result, item => item == expected);
    }

    [Fact]
    public async Task SelectId()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.Id);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Single(
            selector.Select(fullAttribute).Select(r => r.String),
            item => item == "some_id");
        Assert.Empty(selector.Select(noAttribute));
    }

    [Fact]
    public async Task SelectClass()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.Class);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Single(
            selector.Select(fullAttribute).Select(r => r.String),
            item => item == "some_class another_class");
        Assert.Empty(selector.Select(noAttribute));
    }

    [Fact]
    public async Task SelectSplitClasses()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.SplitClasses);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Equal(
            ["some_class", "another_class"],
            selector.Select(fullAttribute).Select(r => r.String));
        Assert.Empty(selector.Select(noAttribute));
    }

    [Fact]
    public async Task SelectHref()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.Href);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Single(
            selector.Select(fullAttribute).Select(r => r.String),
            item => item == "https://some_url_in_japan.go.jp");
        Assert.Empty(selector.Select(noAttribute));
    }

    [Fact]
    public async Task SelectSrc()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.Src);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Single(
            selector.Select(fullAttribute).Select(r => r.String),
            item => item == "https://some_url_in_japan.go.jp/some_pic.jpg");
        Assert.Empty(selector.Select(noAttribute));
    }

    [Fact]
    public async Task SelectTitle()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.Title);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Single(
            selector.Select(fullAttribute).Select(r => r.String),
            item => item == "Some title");
        Assert.Empty(selector.Select(noAttribute));
    }

    [Fact]
    public async Task SelectName()
    {
        AttributeSelector selector = SelectorFactory.CreateSelector(Attr.Name);
        var (fullAttribute, noAttribute) = await CreateElementResourcesAsync();
        Assert.Single(
            selector.Select(fullAttribute).Select(r => r.String),
            item => item == "some_name");
        Assert.Empty(selector.Select(noAttribute));
    }

    private static async Task<(ElementResource fullAttribute, ElementResource noAttribute)>
        CreateElementResourcesAsync()
    {
        var fullAttribute = await CreateElementResourceAsync("full-attribute.html");
        var noAttribute = await CreateElementResourceAsync("no-attribute.html");
        return (fullAttribute, noAttribute);
    }
}