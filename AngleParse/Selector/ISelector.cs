using System.Collections.Generic;
using AngleParse.Resource;

namespace AngleParse.Selector;

public interface ISelector
{
    IEnumerable<IResource> Select(IResource resource);
}