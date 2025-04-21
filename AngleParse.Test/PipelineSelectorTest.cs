using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.StringResource;
using AngleParse.Test.Selector;
using Xunit;

namespace AngleParse.Test;

public class PipelineSelectorTest
{
    private static readonly PipelineSelector ValidSelector =
        new(
            "p > a.mw-redirect",
            Attr.Href,
            new Regex("/wiki/(\\w+)")
        );

    private static readonly PipelineSelector InvalidSelector =
        new(
            "p > a.mw-redirect",
            Attr.Href,
            "*"
        );

    private static readonly PipelineSelector IncludingHashtableSelector =
        new(
            "> div",
            new ValidHashtableSelector()
        );

    private static readonly ElementResource ValidResource = new ValidElementResource();
    private static readonly ElementResource EmptyResource = new EmptyElementResource();
    private static readonly StringResource StringResource = new ValidStringResource();

    [Fact]
    public void InitializingInvalidSelectorThrowsTypeInitializationException()
    {
        Assert.Throws<TypeInitializationException>(() =>
            new PipelineSelector(
                "> div",
                // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
                Int32.MaxValue
            )
        );
    }

    [Fact]
    public void SelectByIncludingHashtableSelectorWorks()
    {
        var actual = IncludingHashtableSelector.Select(ValidResource);
        var enumerable = actual as IResource[] ?? actual.ToArray();
        Assert.Equal(2, enumerable.Length);
        var first = enumerable.First().AsObject();
        var d = first as Hashtable;
        Assert.NotNull(d);

        var redirectLinks = d["redirectLinks"];
        var redirectLinksExpected = new object[] { "Windows_XP_SP2", "Windows_Server_2003_SP1" };
        Assert.Equal(redirectLinksExpected, redirectLinks);

        var cls = d["class"];
        const string clsExpected = "some_class";
        Assert.Equal(cls, clsExpected);

        var reference = d["reference"];
        const string referenceExpected = "[58]";
        Assert.Equal(reference, referenceExpected);
    }

    [Fact]
    public void SelectByInvalidSelectorThrowsInvalidOperationException()
    {
        // To throw exception, you need to evaluate LINQ.
        Assert.Throws<InvalidOperationException>(() =>
            InvalidSelector.Select(ValidResource).ToArray()
        );
    }

    [Fact]
    public void SelectOnEmptyResourceReturnsEmptySeq()
    {
        var actual = ValidSelector.Select(EmptyResource).Select(r => r.AsString());
        Assert.Empty(actual);
    }


    [Fact]
    public void SelectOnStringResourceByElementRequiredPipelineThrowsInvalidOperationException()
    {
        // To throw exception, you need to evaluate LINQ.
        Assert.Throws<InvalidOperationException>(() =>
            ValidSelector.Select(StringResource).ToArray()
        );
    }

    [Fact]
    public void SelectOnValidResourceWorks()
    {
        var expected = new[] { "Windows_XP_SP2", "Windows_Server_2003_SP1", "General_availability" };
        var actual = ValidSelector.Select(ValidResource).Select(r => r.AsString());
        Assert.Equal(expected, actual);
    }
}