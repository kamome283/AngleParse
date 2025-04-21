using System;
using System.Linq;
using AngleSharp;
using AngleSharp.Dom;

namespace AngleParse.Resource;

public class ElementResource : IResource
{
    private readonly IElement _element;

    public ElementResource(IElement element)
    {
        _element = element;
    }

    public ElementResource(string body)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var doc = context.OpenAsync(req => req.Content(body)).Result;
        if (doc.Body is null) throw new InvalidOperationException("The document does not contain a <body> element.");
        _element = doc.Body.ChildElementCount == 1 ? doc.Body.Children.First() : doc.Body;
    }

    public IElement AsElement()
    {
        return _element;
    }

    public string AsString()
    {
        return _element.TextContent;
    }

    public object AsObject()
    {
        return _element.TextContent;
    }
}