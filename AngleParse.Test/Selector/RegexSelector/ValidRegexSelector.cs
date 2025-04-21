using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector;

public class ValidRegexSelector : AngleParse.Selector.RegexSelector
{
    private static readonly Regex Regex = new("(\\w+) (\\d{4})");

    public ValidRegexSelector() : base(Regex)
    {
    }
}