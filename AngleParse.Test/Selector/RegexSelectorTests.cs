using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleParse.Selector;
using Xunit;
using static AngleParse.Test.Helpers.ElementResourceFactory;

namespace AngleParse.Test.Selector;

public sealed class RegexSelectorTests
{
    [Fact]
    public void Create()
    {
        const string pattern = "tag: (.*)";
        var regex = new Regex(pattern);
        var regexSelector = SelectorFactory.CreateSelector(regex);
        Assert.IsType<RegexSelector>(regexSelector);
    }

    [Fact]
    public async Task Select()
    {
        RegexSelector selector = SelectorFactory.CreateSelector(new Regex("""(\w+) (\d{4})"""));
        var resource = await CreateElementResourceAsync("full.html");
        var result = selector.Select(resource).Select(r => r.String);
        var expected = new[]
        {
            "November", "2006", "Server", "2003", "Server", "2008", "August", "2016", "January",
            "2018"
        };
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task NoCapturingSelectorReturnsEmptySequence()
    {
        RegexSelector selector = SelectorFactory.CreateSelector(new Regex("""\w+ \d{4}"""));
        var resource = await CreateElementResourceAsync("full.html");
        var result = selector.Select(resource);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UnmatchedSelectorReturnsEmptySequence()
    {
        RegexSelector selector =
            SelectorFactory.CreateSelector(new Regex("hey, how's your day? (.+)"));
        var resource = await CreateElementResourceAsync("full.html");
        var result = selector.Select(resource);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SelectOnEmptyDocumentReturnsEmptySequence()
    {
        RegexSelector selector = SelectorFactory.CreateSelector(new Regex("""(\w+) (\d{4})"""));
        var resource = await CreateElementResourceAsync("empty.html");
        var result = selector.Select(resource);
        Assert.Empty(result);
    }
}