using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace AngleParse.Resource;

internal sealed record ElementResource(IElement Element) : StringResource(Element.TextContent)
{
    public static async Task<ElementResource> CreateAsync(string content)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var doc = await context.OpenAsync(res => res.Content(content));
        if (doc.Body is null) throw new ArgumentOutOfRangeException(nameof(content));
        var element = doc.Body.ChildElementCount == 1 ? doc.Body.Children.First() : doc.Body;
        return new ElementResource(element);
    }
}