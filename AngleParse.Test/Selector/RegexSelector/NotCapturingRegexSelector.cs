using System.Text.RegularExpressions;

namespace AngleParse.Test.Selector.RegexSelector
{
    public class NotCapturingRegexSelector : AngleParse.Selector.RegexSelector
    {
        private static readonly Regex regex = new Regex("\\w+ \\d{4}");

        public NotCapturingRegexSelector() : base(regex)
        {
        }
    }
}