using System;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;
using AngleSharp.Dom;

namespace AngleParse.Selector
{
    public class AttrSelector : ISelector
    {
        private readonly Attr attr;

        public AttrSelector(Attr attr)
        {
            if (!Enum.IsDefined(typeof(Attr), attr))
            {
                throw new TypeInitializationException(typeof(AttrSelector).FullName,
                    new ArgumentException($"{attr} is an invalid Attr."));
            }

            this.attr = attr;
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            static IEnumerable<IResource> Convert(Attr a, IElement e)
            {
                static IEnumerable<IResource> ToSeq(string? s) =>
                    s != null ? new[] {new StringResource(s)} : new IResource[] { };

                return a switch
                {
                    Attr.Element => new IResource[] {new ElementResource(e)},
                    Attr.InnerHtml => ToSeq(e.InnerHtml),
                    Attr.OuterHtml => ToSeq(e.OuterHtml),
                    Attr.TextContent => ToSeq(e.TextContent),
                    Attr.Id => ToSeq(e.Id),
                    Attr.Class => ToSeq(e.ClassName),
                    Attr.SplitClasses => e.ClassList.Select(c => new StringResource(c)),
                    Attr.Href => ToSeq(e.Attributes["href"]?.Value),
                    Attr.Src => ToSeq(e.Attributes["src"]?.Value),
                    Attr.Title => ToSeq(e.Attributes["title"]?.Value),
                    Attr.Name => ToSeq(e.Attributes["name"]?.Value),
                    _ => throw new InvalidOperationException($"{a} is invalid attr."),
                };
            }

            return Convert(attr, resource.AsElement());
        }
    }
}