using AngleParse.Test.Resource.ElementResource;

namespace AngleParse.Test.Resource.ObjectResource;

public class ElementObjectResource() : AngleParse.Resource.ObjectResource(Obj)
{
    private static readonly object Obj = new ValidElementResource().AsObject();
}