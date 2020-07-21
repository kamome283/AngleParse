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
                    "p > a.mw-redirect",
                    Attr.Href,
                    new Regex("/wiki/(\\w+)"))
            },
            {
                "class", new PipelineSelector(Attr.Class)
            },
        };

        public ValidHashtableSelector() : base(dict)
        {
        }
    }
}