using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal sealed class ScriptBlockSelector(ScriptBlock scriptBlock)
    : ISelector<ObjectResource, ObjectResource>
{
    public IEnumerable<ObjectResource> Select(ObjectResource resource)
    {
        return scriptBlock
            // Set '$_' variable into the script block
            .InvokeWithContext([], [new PSVariable("_", resource.Object)])
            .Select(pso => new ObjectResource(pso));
    }
}