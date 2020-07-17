using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class StringSelector : ISelector
    {
        private readonly string selector;

        public StringSelector(string selector)
        {
            this.selector = selector;
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            return resource.AsElement().QuerySelectorAll(selector).Select(e => new ElementResource(e));
        }
    }
}