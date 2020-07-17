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
            var context = BrowsingContext.New(Configuration.Default);
            var doc = context.OpenAsync(req => req.Content(body)).Result;
            this.element = doc.Body;
        }

        public IElement AsElement()
        {
            return element;
        }

        public string AsString()
        {
            return element.InnerHtml;
        }

        public object AsObject()
        {
            return this;
        }
    }
}