using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class PipelineSelector : ISelector
    {
        private readonly IEnumerable<ISelector> selectors;

        public PipelineSelector(params ISelector[] selectors)
        {
            this.selectors = selectors;
        }

        public PipelineSelector(object[] selectorLikes)
        {
            selectors = selectorLikes.Select(obj => new PipelineSelector(obj));
        }

        public PipelineSelector(object selectorLike)
        {
            switch (selectorLike)
            {
                case string s:
                    selectors = new[] {new StringSelector(s)};
                    break;
                case Regex r:
                    selectors = new[] {new RegexSelector(r)};
                    break;
                case Attr a:
                    selectors = new[] {new AttrSelector(a)};
                    break;
                case ScriptBlock sb:
                    selectors = new[] {new ScriptBlockSelector(sb)};
                    break;
                case Hashtable ht:
                    selectors = new[] {new HashtableSelector(ht)};
                    break;
                case object[] objs:
                    selectors = objs.Select(obj => new PipelineSelector(obj));
                    break;
                default:
                    throw new TypeInitializationException(
                        typeof(PipelineSelector).FullName,
                        new ArgumentException(selectorLike.GetType().FullName)
                    );
            }
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            return selectors.Aggregate(
                new[] {resource} as IEnumerable<IResource>,
                (rs, s) => rs.SelectMany(s.Select)
            );
        }
    }
}