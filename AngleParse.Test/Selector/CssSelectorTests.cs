using System;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public class CssSelectorTests
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
}