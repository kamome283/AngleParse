using System.Collections.Generic;
using AngleParse.Resource;

namespace AngleParse.Selector;

public interface ISelector<in In, out Out> where In : Out where Out : ObjectResource
{
    public IEnumerable<Out> Select(In resource);
}