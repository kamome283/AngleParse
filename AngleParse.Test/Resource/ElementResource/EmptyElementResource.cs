namespace AngleParse.Test.Resource.ElementResource
{
    public class EmptyElementResource : AngleParse.Resource.ElementResource
    {
        private static readonly string body = "";

        public EmptyElementResource() : base(body)
        {
        }
    }
}