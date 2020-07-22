using System;
using AngleSharp.Dom;

namespace AngleParse.Resource
{
    public class ObjectResource : IResource
    {
        private readonly object obj;

        public ObjectResource(object obj)
        {
            this.obj = obj;
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
            return obj;
        }
    }
}