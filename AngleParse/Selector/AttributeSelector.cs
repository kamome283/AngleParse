using System;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal class AttributeSelector(Attr attribute) : ISelector<ElementResource, StringResource>
{
    public IEnumerable<StringResource> Select(ElementResource resource) =>
        attribute switch
        {
            Attr.Element => throw new NotImplementedException(),
            Attr.InnerHtml => WrapToResources(resource.Element.InnerHtml),
            Attr.OuterHtml => WrapToResources(resource.Element.OuterHtml),
            Attr.TextContent => WrapToResources(resource.Element.TextContent),
            Attr.Id => WrapToResources(resource.Element.Id),
            Attr.Class => WrapToResources(resource.Element.ClassName),
            Attr.SplitClasses => resource.Element.ClassList.SelectMany(WrapToResources),
            Attr.Href => WrapToResources(resource.Element.Attributes["href"]?.Value),
            Attr.Src => WrapToResources(resource.Element.Attributes["src"]?.Value),
            Attr.Title => WrapToResources(resource.Element.Attributes["title"]?.Value),
            Attr.Name => WrapToResources(resource.Element.Attributes["name"]?.Value),
            _ => throw new InvalidOperationException($"{attribute} is invalid attr.")
        };

    private static StringResource[] WrapToResources(string? s) =>
        s is null ? [] : [new StringResource(s)];
}