using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector;

public class InvalidRegexSelector() : AngleParse.Selector.RegexSelector(Regex)
{
    // To disable regex inspection on invalid expression, used string variable instead of literal.
    // ReSharper disable once ConvertToConstant.Local
    private static readonly string Expr = "((((";
    private static readonly Regex Regex = new(Expr);
}