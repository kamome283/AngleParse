using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal sealed class AttributeSelector(Attr attribute) : ISelector<ElementResource, StringResource>
{
    public IEnumerable<StringResource> Select(ElementResource resource) =>
        attribute switch
        {
            _ when attribute == Attr.InnerHtml => WrapToResources(resource.Element.InnerHtml),
            _ when attribute == Attr.OuterHtml => WrapToResources(resource.Element.OuterHtml),
            _ when attribute == Attr.TextContent => WrapToResources(resource.Element.TextContent),
            _ when attribute == Attr.Id => WrapToResources(resource.Element.Id),
            _ when attribute == Attr.Class => WrapToResources(resource.Element.ClassName),
            _ when attribute == Attr.SplitClasses => resource.Element.ClassList.SelectMany(
                WrapToResources),
            _ => WrapToResources(resource.Element.Attributes[attribute.Value]?.Value)
        };

    private static StringResource[] WrapToResources(string? s) =>
        s is null ? [] : [new StringResource(s)];
}