using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector;

public class NotMatchingRegexSelector() : AngleParse.Selector.RegexSelector(Regex)
{
    private static readonly Regex Regex = new("fe4r3498njgv49mjb34o9fs");
}