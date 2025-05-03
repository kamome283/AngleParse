using System.Linq;
using System.Threading.Tasks;
using AngleParse.Selector;
using Xunit;
using static AngleParse.Test.Helpers.ElementResourceFactory;

namespace AngleParse.Test.Resource;

public sealed class ElementResourceTests
{
    [Fact]
    public async Task Create()
    {
        var resource = await CreateElementResourceAsync("full.html");
        CssSelector headerSelector = SelectorFactory.CreateSelector("header");
        CssSelector sectionSelector = SelectorFactory.CreateSelector("section");
        CssSelector footerSelector = SelectorFactory.CreateSelector("footer");
        Assert.Single(headerSelector.Select(resource));
        Assert.Equal(4, sectionSelector.Select(resource).Count());
        Assert.Single(footerSelector.Select(resource));
    }

    [Fact]
    public async Task ResourceFromEmptyDocumentContainsEmptyDom()
    {
        var resource = await CreateElementResourceAsync("empty.html");
        Assert.Empty(resource.String);
    }

    [Fact]
    public async Task ResourceFromInvalidDocumentContainsEmptyDom()
    {
        var resource = await CreateElementResourceAsync("invalid.html");
        Assert.Empty(resource.String);
    }

    [Fact]
    public async Task ResourceFromNonHtmlSentenceDoesNotEmpty()
    {
        var resource = await CreateElementResourceAsync("sentence.txt");
        Assert.NotEmpty(resource.String);
    }
}