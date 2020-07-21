using System;
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
            try
            {
                this.regex = regex;
            }
            catch (TypeInitializationException e)
            {
                throw new TypeInitializationException(typeof(RegexSelector).FullName, e);
            }
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            var body = resource.AsString();
            return regex.Matches(body).SelectMany(StringResource.FromMatch);
        }
    }
}