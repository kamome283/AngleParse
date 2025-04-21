using System;
using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.AttrElementResource;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.ObjectResource;
using AngleParse.Test.Resource.StringResource;
using AngleSharp.Dom;
using Xunit;

namespace AngleParse.Test;

public class AttrSelectorTest
{
    private static readonly ElementResource ValidResource = new ValidAttrElementResource();
    private static readonly ElementResource EmptyResource = new EmptyAttrElementResource();
    private static readonly ElementResource BodyResource = new ValidElementResource();
    private static readonly StringResource StringResource = new ValidStringResource();
    private static readonly ObjectResource ObjResource = new ElementObjectResource();

    private static void AttrTest(Attr attr, IResource resource, params string[] expected)
    {
        var selector = new AttrSelector(attr);
        var actual = selector.Select(resource).Select(r => r.AsString());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InitializingSelectorWithInvalidAttrThrowsTypeInitializationException()
    {
        Assert.Throws<TypeInitializationException>(() =>
            // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
            new AttrSelector((Attr)Int32.MaxValue)
        );
    }

    [Fact]
    public void SelectClassWorks()
    {
        AttrTest(Attr.Class, ValidResource, "some_class another_class");
    }

    [Fact]
    public void SelectElementWorks()
    {
        var selector = new AttrSelector(Attr.Element);
        var actual = selector.Select(ValidResource);
        Assert.Single(actual);
        var first = actual.First();
        Assert.IsAssignableFrom<IElement>(first.AsObject());
    }

    [Fact]
    public void SelectHrefWorks()
    {
        AttrTest(Attr.Href, ValidResource, "https://some_url_in_japan.go.jp");
    }

    [Fact]
    public void SelectIdWorks()
    {
        AttrTest(Attr.Id, ValidResource, "some_id");
    }

    [Fact]
    public void SelectInnerHtmlWorks()
    {
        AttrTest(Attr.InnerHtml, EmptyResource, @"Some text and <a>some link</a>.");
    }

    [Fact]
    public void SelectNameWorks()
    {
        AttrTest(Attr.Name, ValidResource, "some_name");
    }

    [Fact]
    public void SelectNonExistAttributesReturnsEmptySequence()
    {
        foreach (var attr in Enum.GetValues(typeof(Attr)).Cast<Attr>()
                     .Except([Attr.Element, Attr.InnerHtml, Attr.OuterHtml, Attr.TextContent]))
            AttrTest(attr, EmptyResource);
    }

    [Fact]
    public void SelectOnBodyElementAttributesReturnsEmptySequence()
    {
        foreach (var attr in Enum.GetValues(typeof(Attr)).Cast<Attr>()
                     .Except([Attr.Element, Attr.InnerHtml, Attr.OuterHtml, Attr.TextContent]))
            AttrTest(attr, BodyResource);
    }

    [Fact]
    public void SelectOnObjResourceThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            AttrTest(Attr.InnerHtml, ObjResource)
        );
    }

    [Fact]
    public void SelectOnStringResourceThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            AttrTest(Attr.Id, StringResource)
        );
    }

    [Fact]
    public void SelectOuterHtmlWorks()
    {
        AttrTest(Attr.OuterHtml, EmptyResource, @"<p>Some text and <a>some link</a>.</p>");
    }

    [Fact]
    public void SelectSplitClassesWorks()
    {
        AttrTest(Attr.SplitClasses, ValidResource, "some_class", "another_class");
    }

    [Fact]
    public void SelectSrcWorks()
    {
        AttrTest(Attr.Src, ValidResource, "https://some_url_in_japan.go.jp/some_pic.jpg");
    }

    [Fact]
    public void SelectTextContentWorks()
    {
        AttrTest(Attr.TextContent, EmptyResource, "Some text and some link.");
    }

    [Fact]
    public void SelectTitleWorks()
    {
        AttrTest(Attr.Title, ValidResource, "Some title");
    }
}