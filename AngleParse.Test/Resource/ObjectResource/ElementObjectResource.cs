using AngleParse.Test.Resource.ElementResource;

namespace AngleParse.Test.Resource.ObjectResource
{
    public class ElementObjectResource : AngleParse.Resource.ObjectResource
    {
        private static readonly object obj = new ValidElementResource().AsObject();

        public ElementObjectResource() : base(obj)
        {
        }
    }
}