using System;
using System.Linq;
using System.Threading.Tasks;
using AngleParse.Selector;
using Xunit;
using static AngleParse.Test.Helpers.ElementResourceFactory;

namespace AngleParse.Test.Selector;

public sealed class CssSelectorTests
{
    [Fact]
    public void CreateWithValidSelector()
    {
        const string validCssSelectorExpr = "div > p";
        var cssSelector = SelectorFactory.CreateSelector(validCssSelectorExpr);
        Assert.IsType<CssSelector>(cssSelector);
    }

    [Fact]
    public void CreatingWithInvalidSelectorThrowsException()
    {
        const string invalidCssSelectorExpr = "div > p >";
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SelectorFactory.CreateSelector(invalidCssSelectorExpr));
    }

    [Fact]
    public async Task Select()
    {
        CssSelector selector = SelectorFactory.CreateSelector("section#fragment p > a.mw-redirect");
        var resource = await CreateElementResourceAsync("full.html");
        var result = selector.Select(resource).Select(r => r.String);
        var expected = new[]
            { "Windows XP SP2", "Windows Server 2003 SP1", "general availability" };
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task UnmatchedSelectorReturnsEmptySequence()
    {
        CssSelector selector = SelectorFactory.CreateSelector("section#fragment div > a > p > div");
        var resource = await CreateElementResourceAsync("full.html");
        var result = selector.Select(resource);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SelectOnEmptyDocumentReturnsEmptySequence()
    {
        CssSelector selector = SelectorFactory.CreateSelector("div");
        var resource = await CreateElementResourceAsync("empty.html");
        var result = selector.Select(resource);
        Assert.Empty(result);
    }
}