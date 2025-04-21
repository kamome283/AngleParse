using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector;

public class PipelineSelector : ISelector
{
    private readonly ISelector[] _selectors;

    private PipelineSelector(ISelector[] selectors)
    {
        _selectors = selectors;
    }

    public PipelineSelector(params object[] selectorLikes)
    {
        _selectors = selectorLikes.Select(InitSelector).ToArray();
    }

    public IEnumerable<IResource> Select(IResource resource)
    {
        return _selectors.Aggregate(
            new[] { resource } as IEnumerable<IResource>,
            (rs, s) => rs.SelectMany(s.Select)
        );
    }

    private static ISelector InitSelector(object selectorLike)
    {
        return selectorLike switch
        {
            string s => new StringSelector(s),
            Regex r => new RegexSelector(r),
            Attr a => new AttrSelector(a),
            ScriptBlock sb => new ScriptBlockSelector(sb),
            Hashtable ht => new HashtableSelector(ht),
            ISelector sl => sl,
            object[] objs => new PipelineSelector(objs.Select(InitSelector).ToArray()),
            _ => throw new TypeInitializationException(typeof(PipelineSelector).FullName,
                new ArgumentException(selectorLike.GetType().FullName))
        };
    }
}