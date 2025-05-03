using System;
using System.Collections.Generic;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal sealed class FuncSelector<In, Out>(Func<In, IEnumerable<Out>> func) : ISelector<In, Out>
    where In : Out
    where Out : ObjectResource
{
    public IEnumerable<Out> Select(In resource) => func(resource);
}