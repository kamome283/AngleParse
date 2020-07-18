namespace AngleParse.Test.Resource.AttrElementResource
{
    public class EmptyAttrElementResource : AngleParse.Resource.ElementResource
    {
        private const string body = @"<p>Some text and <a>some link</a>.</p>";

        public EmptyAttrElementResource() : base(body)
        {
        }
    }
}