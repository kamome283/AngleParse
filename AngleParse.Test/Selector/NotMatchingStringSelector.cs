using AngleParse.Selector;

namespace AngleParse.Test.Selector
{
    public class NotMatchingStringSelector : StringSelector
    {
        public NotMatchingStringSelector() : base("div > a > p > div")
        {
        }
    }
}