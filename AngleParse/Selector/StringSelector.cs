using System;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;
using AngleSharp.Dom;

namespace AngleParse.Selector;

public class StringSelector : ISelector
{
    private readonly string _selector;

    public StringSelector(string selector)
    {
        // Validate selector
        try
        {
            var er = new ElementResource("");
            er.AsElement().QuerySelectorAll(selector);
        }
        catch (DomException e)
        {
            throw new TypeInitializationException(typeof(StringSelector).FullName, e);
        }

        _selector = selector;
    }

    public IEnumerable<IResource> Select(IResource resource)
    {
        return resource.AsElement().QuerySelectorAll(_selector).Select(e => new ElementResource(e));
    }
}