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
            IEnumerable<IResource> convert(Attr a, IElement e)
            {
                IEnumerable<IResource> toSeq(string? s) =>
                    s != null ? new[] {new StringResource(s)} : new IResource[] { };

                return a switch
                {
                    Attr.InnerHtml => toSeq(e.InnerHtml),
                    Attr.OuterHtml => toSeq(e.OuterHtml),
                    Attr.TextContent => toSeq(e.TextContent),
                    Attr.Id => toSeq(e.Id),
                    Attr.Class => toSeq(e.ClassName),
                    Attr.SplitClasses => e.ClassList.Select(c => new StringResource(c)),
                    Attr.Href => toSeq(e.Attributes["href"]?.Value),
                    Attr.Src => toSeq(e.Attributes["src"]?.Value),
                    Attr.Title => toSeq(e.Attributes["title"]?.Value),
                    Attr.Name => toSeq(e.Attributes["name"]?.Value),
                    _ => throw new InvalidOperationException($"{a} is invalid attr."),
                };
            }

            return convert(attr, resource.AsElement());
        }
    }
}