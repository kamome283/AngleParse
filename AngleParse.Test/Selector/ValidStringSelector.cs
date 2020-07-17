using AngleParse.Selector;

namespace AngleParse.Test.Selector
{
    public class ValidStringSelector : StringSelector
    {
        public ValidStringSelector() : base("p > a.mw-redirect")
        {
        }
    }
}