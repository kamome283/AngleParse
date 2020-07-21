using System.Collections.Generic;
using System.Text.RegularExpressions;
using AngleParse.Selector;

namespace AngleParse.Test.Selector
{
    public class ValidHashtableSelector : HashtableSelector
    {
        private static readonly Dictionary<object, ISelector> dict = new Dictionary<object, ISelector>
        {
            {
                "redirectLinks", new PipelineSelector(
                    new AngleParse.Selector.StringSelector("p > a.mw-redirect"),
                    new AttrSelector(Attr.Href),
                    new AngleParse.Selector.RegexSelector(new Regex("/wiki/(\\w+)")))
            },
            {
                "class", new AttrSelector(Attr.Class)
            },
        };

        public ValidHashtableSelector() : base(dict)
        {
        }
    }
}