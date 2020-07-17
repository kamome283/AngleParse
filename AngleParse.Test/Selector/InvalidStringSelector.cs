namespace AngleParse.Test.Selector
{
    public class InvalidStringSelector : AngleParse.Selector.StringSelector
    {
        public InvalidStringSelector() : base("fjldsel><eeer.::<%%")
        {
        }
    }
}