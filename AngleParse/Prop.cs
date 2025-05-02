namespace AngleParse;

public sealed record Prop(string Value)
{
    public static Prop Element => new(string.Empty);
    public static Prop AttributesTable => new("!AttributesTable");
}