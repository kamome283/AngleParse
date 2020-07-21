using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;

namespace AngleParse.Selector
{
    public class HashtableSelector : ISelector
    {
        private readonly Dictionary<object, ISelector> table;

        protected HashtableSelector(Dictionary<object, ISelector> table)
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

            // I decided to return PSCustomObject, but there is no way to create it from C#.
            // Using ScriptBlock.Create("[pscustomobject] $_") may work well but disrupts testability,
            // thus decided not to create PSCustomObject here but in the top level.
            // That will cover most cases and keeps testability.
            var ht = new Hashtable(evaluated);
            return new IResource[] {new ObjectResource(ht)};
        }
    }
}