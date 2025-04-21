namespace AngleParse.Test.Resource.AttrElementResource;

public class ValidAttrElementResource : AngleParse.Resource.ElementResource
{
    private const string Body = @"
    <a
        id=""some_id""
        class=""some_class another_class""
        href=""https://some_url_in_japan.go.jp""
        src=""https://some_url_in_japan.go.jp/some_pic.jpg""
        title=""Some title""
        name=""some_name""
    >
        some link
    </a>
";

    public ValidAttrElementResource() : base(Body)
    {
    }
}