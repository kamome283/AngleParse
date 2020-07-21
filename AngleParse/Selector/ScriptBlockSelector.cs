using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class ScriptBlockSelector : ISelector
    {
        private readonly ScriptBlock sb;

        public ScriptBlockSelector(ScriptBlock sb)
        {
            this.sb = sb != null
                ? sb
                : throw new TypeInitializationException(nameof(ScriptBlockSelector),
                    new NullReferenceException());
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            return sb.InvokeWithContext(
                new Dictionary<string, ScriptBlock>(),
                new List<PSVariable> {new PSVariable("_", resource.AsObject())}
            ).Select(pso => new ObjectResource(pso));
        }
    }
}