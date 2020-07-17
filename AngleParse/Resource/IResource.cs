using AngleSharp.Dom;

namespace AngleParse.Resource
{
    public interface IResource
    {
        IElement AsElement();
        string AsString();
        object AsObject();
    }
}