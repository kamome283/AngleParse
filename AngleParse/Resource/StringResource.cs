using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace AngleParse.Resource;

public class StringResource : IResource
{
    private readonly string _str;

    public StringResource(string str)
    {
        _str = str;
    }

    public IElement AsElement()
    {
        throw new InvalidOperationException("Cannot operate HTML element required operation on string.");
    }

    public string AsString()
    {
        return _str;
    }

    public object AsObject()
    {
        return _str;
    }

    public static IEnumerable<StringResource> FromMatch(Match m)
    {
        return m.Groups.Cast<Group>().Skip(1).Select(c => new StringResource(c.Value));
    }
}