using System;
using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.ObjectResource;
using AngleParse.Test.Resource.StringResource;
using AngleParse.Test.Selector.RegexSelector;
using Xunit;

namespace AngleParse.Test;

public class RegexSelectorTest
{
    // Should throw exception
    // private static readonly RegexSelector invalidSelector = new InvalidRegexSelector();
    private static readonly RegexSelector ValidSelector = new ValidRegexSelector();
    private static readonly RegexSelector NotMatchingSelector = new NotMatchingRegexSelector();
    private static readonly RegexSelector NotCapturingSelector = new NotCapturingRegexSelector();

    private static readonly ElementResource ValidResource = new ValidElementResource();
    private static readonly ElementResource InvalidResource = new InvalidElementResource();
    private static readonly StringResource StringResource = new ValidStringResource();
    private static readonly ObjectResource ObjResource = new ElementObjectResource();

    [Fact]
    public void InitializingInvalidSelectorThrowsTypeInitializationException()
    {
        Assert.Throws<TypeInitializationException>(() =>
            new InvalidRegexSelector()
        );
    }

    [Fact]
    public void SelectByNotCapturingSelectorReturnsEmptySequence()
    {
        var actual = NotCapturingSelector.Select(StringResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }

    [Fact]
    public void SelectByNotMatchingSelectorReturnsEmptySequence()
    {
        var actual = NotMatchingSelector.Select(StringResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }

    [Fact]
    public void SelectOnInvalidElementResourceWorks()
    {
        var actual = ValidSelector.Select(InvalidResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }

    [Fact]
    public void SelectOnObjResourceThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            ValidSelector.Select(ObjResource));
    }

    [Fact]
    public void SelectOnValidElementResourceWorks()
    {
        var expected = new[]
        {
            "November", "2006", "Server", "2003", "Server", "2008", "August", "2016", "January", "2018"
        };
        var actual = ValidSelector.Select(ValidResource).Select(r => r.AsString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SelectOnValidStringResourceWorks()
    {
        var expected = new[]
        {
            "November", "2006", "Server", "2003", "Server", "2008", "Server", "2016", "Server", "2008", "Server",
            "2008", "Server", "2012", "Server", "2012"
        };
        var actual = ValidSelector.Select(StringResource).Select(r => r.AsString());
        Assert.Equal(expected, actual);
    }
}