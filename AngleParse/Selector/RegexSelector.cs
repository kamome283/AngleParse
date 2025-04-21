using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector;

public class RegexSelector : ISelector
{
    private readonly Regex _regex;

    public RegexSelector(Regex regex)
    {
        try
        {
            _regex = regex;
        }
        catch (TypeInitializationException e)
        {
            throw new TypeInitializationException(typeof(RegexSelector).FullName, e);
        }
    }

    public IEnumerable<IResource> Select(IResource resource)
    {
        var body = resource.AsString();
        return _regex.Matches(body).SelectMany(StringResource.FromMatch);
    }
}