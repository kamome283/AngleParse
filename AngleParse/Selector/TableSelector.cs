using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal sealed class TableSelector<In>(IDictionary<object, ISelector<In, ObjectResource>> table)
    : ISelector<In, ObjectResource> where In : ObjectResource
{
    public IEnumerable<ObjectResource> Select(In resource)
    {
        Dictionary<object, object?> evaluated = [];
        foreach (var (key, selector) in table)
            evaluated[key] = Unify(selector.Select(resource).Select(r => r.Object).ToArray());
        PSObject psObject = new Hashtable(evaluated);
        return [new ObjectResource(psObject)];
    }

    private static object? Unify(object[] objects) =>
        objects.Length switch
        {
            0 => null,
            1 => objects[0],
            _ => objects
        };
}