using System.Linq;
using AngleSharp;
using AngleSharp.Dom;

namespace AngleParse.Resource
{
    public class ElementResource : IResource
    {
        private readonly IElement element;

        public ElementResource(IElement element)
        {
            this.element = element;
        }

        public ElementResource(string body)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var doc = context.OpenAsync(req => req.Content(body)).Result;
            element = doc.Body.ChildElementCount == 1 ? doc.Body.Children.First() : doc.Body;
        }

        public IElement AsElement()
        {
            return element;
        }

        public string AsString()
        {
            return element.TextContent;
        }

        public object AsObject()
        {
            return element.TextContent;
        }
    }
}