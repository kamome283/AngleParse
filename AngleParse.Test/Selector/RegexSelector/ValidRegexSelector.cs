using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector;

public class ValidRegexSelector() : AngleParse.Selector.RegexSelector(Regex)
{
    private static readonly Regex Regex = new("""(\w+) (\d{4})""");
}