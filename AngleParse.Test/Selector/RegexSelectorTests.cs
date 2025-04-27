using System.Text.RegularExpressions;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public class RegexSelectorTests
{
    [Fact]
    public void Create()
    {
        const string pattern = "tag: (.*)";
        var regex = new Regex(pattern);
        var regexSelector = SelectorFactory.CreateSelector(regex);
        Assert.IsType<RegexSelector>(regexSelector);
    }
}