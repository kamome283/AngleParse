namespace AngleParse.Test.Selector.StringSelector;

public class InvalidStringSelector : AngleParse.Selector.StringSelector
{
    // ReSharper disable once StringLiteralTypo
    public InvalidStringSelector() : base("fjldsel><ee.::<%%")
    {
    }
}