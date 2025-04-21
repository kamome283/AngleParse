using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector;

public class HashtableSelector : ISelector
{
    private readonly Dictionary<object, ISelector> _table;

    protected HashtableSelector(Dictionary<object, ISelector> table)
    {
        _table = table;
    }

    public HashtableSelector(Hashtable hashtable)
    {
        _table = hashtable.Cast<DictionaryEntry>().ToDictionary(
            p => p.Key,
            p => new PipelineSelector(p.Value) as ISelector
        );
    }

    public IEnumerable<IResource> Select(IResource resource)
    {
        var evaluated = _table.ToDictionary(
            p => p.Key,
            // Evaluate earlier for ease of use from PowerShell
            // Unify array to simulate PowerShell's behavior.
            p => Unify(p.Value.Select(resource).Select(r => r.AsObject()).ToArray())
        );

        // I decided to return PSCustomObject, but there is no way to create it from C#.
        // Using ScriptBlock.Create("[pscustomobject] $_") may work well but disrupts testability,
        // thus decided not to create PSCustomObject here but in the top level.
        // That will cover most cases and keeps testability.
        var ht = new Hashtable(evaluated);
        return [new ObjectResource(ht)];
    }

    private static object Unify(object[] objs)
    {
        return objs.Length switch
        {
            0 => null,
            1 => objs.First(),
            _ => objs
        };
    }
}