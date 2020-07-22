using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class PipelineSelector : ISelector
    {
        private readonly ISelector[] selectors;

        private PipelineSelector(ISelector[] selectors)
        {
            this.selectors = selectors;
        }

        public PipelineSelector(params object[] selectorLikes)
        {
            selectors = selectorLikes.Select(InitSelector).ToArray();
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            return selectors.Aggregate(
                new[] {resource} as IEnumerable<IResource>,
                (rs, s) => rs.SelectMany(s.Select)
            );
        }

        private static ISelector InitSelector(object selectorLike)
        {
            switch (selectorLike)
            {
                case string s:
                    return new StringSelector(s);
                case Regex r:
                    return new RegexSelector(r);
                case Attr a:
                    return new AttrSelector(a);
                case ScriptBlock sb:
                    return new ScriptBlockSelector(sb);
                case Hashtable ht:
                    return new HashtableSelector(ht);
                case ISelector sl:
                    return sl;
                case object[] objs:
                    return new PipelineSelector(objs.Select(InitSelector).ToArray());
                default:
                    throw new TypeInitializationException(
                        typeof(PipelineSelector).FullName,
                        new ArgumentException(selectorLike.GetType().FullName)
                    );
            }
        }
    }
}