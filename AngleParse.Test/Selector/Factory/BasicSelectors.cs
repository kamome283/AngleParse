using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector.Factory;

public class BasicSelectors
{
    [Fact]
    public void CanCreateCssSelectorWithValidExpression()
    {
        const string validCssSelectorExpr = "div > p";
        var cssSelector = SelectorFactory.CreateSelector(validCssSelectorExpr);
        Assert.IsType<CssSelector>(cssSelector);
    }

    [Fact]
    public void CssSelectorWithInvalidExpressionWillThrowException()
    {
        const string invalidCssSelectorExpr = "div > p >";
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SelectorFactory.CreateSelector(invalidCssSelectorExpr));
    }

    [Fact]
    public void CanCreateRegexSelector()
    {
        const string pattern = "tag: (.*)";
        var regex = new Regex(pattern);
        var regexSelector = SelectorFactory.CreateSelector(regex);
        Assert.IsType<RegexSelector>(regexSelector);
    }

    [Fact]
    public void CanCreateAttributeSelectorWithValidAttribute()
    {
        const Attr attribute = Attr.Href;
        var attributeSelector = SelectorFactory.CreateSelector(attribute);
        Assert.IsType<AttributeSelector>(attributeSelector);
    }

    [Fact]
    public void CanCreateScriptBlockSelector()
    {
        var scriptBlock = ScriptBlock.Create("{ $_.Name -eq 'test' }");
        var scriptBlockSelector = SelectorFactory.CreateSelector(scriptBlock);
        Assert.IsType<ScriptBlockSelector>(scriptBlockSelector);
    }
}