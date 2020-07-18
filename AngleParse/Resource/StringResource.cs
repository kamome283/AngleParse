using System;
using AngleSharp.Dom;

namespace AngleParse.Resource
{
    public class StringResource : IResource
    {
        private readonly string str;

        public StringResource(string str)
        {
            this.str = str;
        }

        public IElement AsElement()
        {
            throw new InvalidOperationException("Cannot operate HTML element required operation on string.");
        }

        public string AsString()
        {
            return str;
        }

        public object AsObject()
        {
            return str;
        }
    }
}