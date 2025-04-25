using System;
using System.Collections.Generic;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal class FuncSelector<In, Out>(Func<In, IEnumerable<Out>> func) : ISelector<In, Out>
    where In : Out
    where Out : ObjectResource
{
    public FuncSelector(ISelector<In, Out> selector) : this(selector.Select) { }
    public static FuncSelector<In, In> Identity => new(x => [x]);
    public IEnumerable<Out> Select(In resource) => func(resource);
}