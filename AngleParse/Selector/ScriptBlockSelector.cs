using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;

namespace AngleParse.Selector;

public class ScriptBlockSelector(ScriptBlock sb) : ISelector
{
    private readonly ScriptBlock _sb = sb ?? throw new TypeInitializationException(
        nameof(ScriptBlockSelector),
        new NullReferenceException()
    );

    public IEnumerable<IResource> Select(IResource resource)
    {
        return _sb.InvokeWithContext(
            new Dictionary<string, ScriptBlock>(),
            [new PSVariable("_", resource.AsObject())]
        ).Select(pso => new ObjectResource(pso));
    }
}