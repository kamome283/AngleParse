using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector
{
    public class InvalidRegexSelector : AngleParse.Selector.RegexSelector
    {
        // To disable regex inspection on invalid expression, used string variable instead of literal.
        // ReSharper disable once ConvertToConstant.Local
        private static readonly string expr = "((((";
        private static readonly Regex regex = new Regex(expr);

        public InvalidRegexSelector() : base(regex)
        {
        }
    }
}