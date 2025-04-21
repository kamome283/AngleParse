using System;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;
using AngleSharp.Dom;

namespace AngleParse.Selector;

public class AttrSelector : ISelector
{
    private readonly Attr _attr;

    public AttrSelector(Attr attr)
    {
        if (!Enum.IsDefined(typeof(Attr), attr))
            throw new TypeInitializationException(typeof(AttrSelector).FullName,
                new ArgumentException($"{attr} is an invalid Attr."));

        _attr = attr;
    }

    public IEnumerable<IResource> Select(IResource resource)
    {
        IEnumerable<IResource> Convert(Attr a, IElement e)
        {
            IEnumerable<IResource> ToSeq(string s)
            {
                return s != null ? new[] { new StringResource(s) } : new IResource[] { };
            }

            switch (a)
            {
                case Attr.Element:
                    return [new ObjectResource(e)];
                case Attr.InnerHtml:
                    return ToSeq(e.InnerHtml);
                case Attr.OuterHtml:
                    return ToSeq(e.OuterHtml);
                case Attr.TextContent:
                    return ToSeq(e.TextContent);
                case Attr.Id:
                    return ToSeq(e.Id);
                case Attr.Class:
                    return ToSeq(e.ClassName);
                case Attr.SplitClasses:
                    return e.ClassList.Select(c => new StringResource(c));
                case Attr.Href:
                    return ToSeq(e.Attributes["href"]?.Value);
                case Attr.Src:
                    return ToSeq(e.Attributes["src"]?.Value);
                case Attr.Title:
                    return ToSeq(e.Attributes["title"]?.Value);
                case Attr.Name:
                    return ToSeq(e.Attributes["name"]?.Value);
                default:
                    throw new InvalidOperationException($"{a} is invalid attr.");
            }
        }

        return Convert(_attr, resource.AsElement());
    }
}