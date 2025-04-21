using System;
using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.ObjectResource;
using AngleParse.Test.Resource.StringResource;
using AngleParse.Test.Selector.StringSelector;
using Xunit;

namespace AngleParse.Test;

public class StringSelectorTest
{
    // Should throw TypeInitializationException
    // private static readonly StringSelector invalidSelector = new InvalidStringSelector();
    private static readonly StringSelector ValidSelector = new ValidStringSelector();
    private static readonly StringSelector NotMatchingSelector = new NotMatchingStringSelector();

    // AngleSharp tries to parse even broken HTML, so that never throws exception.
    private static readonly ElementResource InvalidResource = new InvalidElementResource();
    private static readonly ElementResource ValidResource = new ValidElementResource();
    private static readonly ElementResource EmptyResource = new EmptyElementResource();
    private static readonly StringResource StringResource = new ValidStringResource();
    private static readonly ObjectResource ObjResource = new ElementObjectResource();

    [Fact]
    public void InitializingInvalidSelectorThrowsTypeInitializationException()
    {
        Assert.Throws<TypeInitializationException>(() =>
            new InvalidStringSelector()
        );
    }

    [Fact]
    public void SelectByNotMatchingSelectorReturnsEmptySequence()
    {
        var actual = NotMatchingSelector.Select(ValidResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }

    [Fact]
    public void SelectByValidSelectorWorks()
    {
        var expected = new[] { "Windows XP SP2", "Windows Server 2003 SP1", "general availability" };
        var actual = ValidSelector.Select(ValidResource).Select(r => r.AsString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SelectOnEmptyResourceReturnsEmptySequence()
    {
        var actual = ValidSelector.Select(EmptyResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }

    [Fact]
    public void SelectOnInvalidResourceReturnsEmptySequence()
    {
        var actual = ValidSelector.Select(InvalidResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }

    [Fact]
    public void SelectOnObjResourceThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            ValidSelector.Select(ObjResource)
        );
    }

    [Fact]
    public void SelectOnStringResourceThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            ValidSelector.Select(StringResource)
        );
    }
}