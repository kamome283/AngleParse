using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<IResource> Select(IResource resource)
        {
            return selectors.Aggregate(
                new[] {resource} as IEnumerable<IResource>,
                (rs, s) => rs.SelectMany(s.Select)
            );
        }
    }
}