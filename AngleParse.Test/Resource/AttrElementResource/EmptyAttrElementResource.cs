namespace AngleParse.Test.Resource.AttrElementResource;

public class EmptyAttrElementResource() : AngleParse.Resource.ElementResource(Body)
{
    private const string Body = "<p>Some text and <a>some link</a>.</p>";
}