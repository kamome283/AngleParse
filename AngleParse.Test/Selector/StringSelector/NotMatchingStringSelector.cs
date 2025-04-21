namespace AngleParse.Test.Selector.StringSelector;

public class NotMatchingStringSelector : AngleParse.Selector.StringSelector
{
    public NotMatchingStringSelector() : base("div > a > p > div")
    {
    }
}