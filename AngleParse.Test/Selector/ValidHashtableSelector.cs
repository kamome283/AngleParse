using System.Collections.Generic;
using System.Text.RegularExpressions;
using AngleParse.Selector;

namespace AngleParse.Test.Selector;

public class ValidHashtableSelector() : HashtableSelector(Dict)
{
    private static readonly Dictionary<object, ISelector> Dict = new()
    {
        {
            "redirectLinks", new PipelineSelector(
                "p > a.mw-redirect",
                Attr.Href,
                new Regex("/wiki/(\\w+)"))
        },
        {
            "class", new PipelineSelector(Attr.Class)
        },
        {
            "reference", new PipelineSelector("sup.reference > a")
        }
    };
}