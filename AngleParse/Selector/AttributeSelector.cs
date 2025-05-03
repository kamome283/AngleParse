using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal sealed class AttributeSelector(Attr attr) : ISelector<ElementResource, StringResource>
{
    public IEnumerable<StringResource> Select(ElementResource resource) =>
        attr switch
        {
            _ when attr == Attr.InnerHtml => WrapToResources(resource.Element.InnerHtml),
            _ when attr == Attr.OuterHtml => WrapToResources(resource.Element.OuterHtml),
            _ when attr == Attr.TextContent => WrapToResources(resource.Element.TextContent),
            _ when attr == Attr.Id => WrapToResources(resource.Element.Id),
            _ when attr == Attr.Class => WrapToResources(resource.Element.ClassName),
            _ when attr == Attr.SplitClasses => resource.Element.ClassList.SelectMany(
                WrapToResources),
            _ => WrapToResources(resource.Element.Attributes[attr.Value]?.Value)
        };

    private static StringResource[] WrapToResources(string? s) =>
        s is null ? [] : [new StringResource(s)];
}