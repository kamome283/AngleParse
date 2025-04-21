using System;
using AngleSharp.Dom;

namespace AngleParse.Resource;

public class ObjectResource(object obj) : IResource
{
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
        return obj;
    }
}