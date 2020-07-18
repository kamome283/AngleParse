using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class RegexSelector : ISelector
    {
        private readonly Regex regex;

        public RegexSelector(Regex regex)
        {
            this.regex = regex;
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            IEnumerable<IResource> collectConvert(Match m) =>
                m.Groups.Skip(1).Select(c => new StringResource(c.Value));

            var body = resource.AsString();
            return regex.Matches(body).SelectMany(collectConvert);
        }
    }
}