using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class HashtableSelector : ISelector
    {
        private readonly Dictionary<object, ISelector> table;

        public HashtableSelector(Dictionary<object, ISelector> table)
        {
            this.table = table;
        }

        public HashtableSelector(Hashtable hashtable)
        {
            table = hashtable.Cast<DictionaryEntry>().ToDictionary(
                p => p.Key,
                p => new PipelineSelector(p.Value) as ISelector
            );
        }

        public IEnumerable<IResource> Select(IResource resource)
        {
            var evaluated = table.ToDictionary(
                p => p.Key,
                // Evaluate earlier for ease of use from PowerShell 
                p => p.Value.Select(resource).Select(r => r.AsObject()).ToArray()
            );
            return new IResource[] {new ObjectResource(evaluated)};
        }
    }
}