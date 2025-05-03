using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal sealed class RegexSelector(Regex regex) : ISelector<StringResource, StringResource>
{
    public IEnumerable<StringResource> Select(StringResource resource) =>
        regex.Matches(resource.String).SelectMany(GetGroupedValue);

    private static IEnumerable<StringResource> GetGroupedValue(Match match) =>
        match
            .Groups
            .Cast<Group>()
            .Skip(1) // Skip the first group which is the whole match
            .Select(group => new StringResource(group.Value));
}