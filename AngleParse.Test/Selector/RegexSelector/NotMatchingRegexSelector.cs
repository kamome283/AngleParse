using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector;

public class NotMatchingRegexSelector : AngleParse.Selector.RegexSelector
{
    private static readonly Regex Regex = new("fe4r3498njgv49mjb34o9fs");

    public NotMatchingRegexSelector() : base(Regex)
    {
    }
}