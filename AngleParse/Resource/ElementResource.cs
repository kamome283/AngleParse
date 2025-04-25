using AngleSharp.Dom;

namespace AngleParse.Resource;

internal class ElementResource(IElement element) : StringResource(element.TextContent)
{
    public IElement Element => element;
}