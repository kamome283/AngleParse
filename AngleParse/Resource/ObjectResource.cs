using System;
using AngleSharp.Dom;

namespace AngleParse.Resource;

public class ObjectResource : IResource
{
    private readonly object _obj;

    public ObjectResource(object obj)
    {
        _obj = obj;
    }

    public IElement AsElement()
    {
        throw new InvalidOperationException("Cannot operate HTML element required operation on object.");
    }

    public string AsString()
    {
        throw new InvalidOperationException("Cannot operate string required operation on object");
    }

    public object AsObject()
    {
        return _obj;
    }
}