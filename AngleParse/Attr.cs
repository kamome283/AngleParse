namespace AngleParse;

public sealed record Attr(string Value)
{
    public static Attr InnerHtml => new("'<>InnerHtml");
    public static Attr OuterHtml => new("'<>OuterHtml");
    public static Attr TextContent => new("'<>TextContent");
    public static Attr Id => new("'<>Id");
    public static Attr ClassName => new("'<>class");
    public static Attr SplitClasses => new("'<>splitClasses");
    public static Attr Href => new("href");
    public static Attr Src => new("src");
    public static Attr Title => new("title");
    public static Attr Name => new("name");
}