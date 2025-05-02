using System;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;
using AngleSharp.Dom;

namespace AngleParse.Selector;

internal sealed class PropertySelector(Prop prop) : ISelector<ElementResource, ObjectResource>
{
    public IEnumerable<ObjectResource> Select(ElementResource resource)
    {
        if (prop == Prop.Element)
            return [new ObjectResource(resource.Element)];
        if (prop == Prop.AttributesTable)
        {
            var attributesTable = resource
                .Element
                .Attributes
                .ToDictionary(a => a.Name, a => a.Value);
            // TODO: Search if Dictionary type is easy to use in PowerShell
            // and if not, implement some measures to make it easier to use.
            return [new ObjectResource(attributesTable)];
        }

        var accessedValue = DynamicAccess(resource.Element, prop.Value);
        return accessedValue is null ? [] : [new ObjectResource(accessedValue)];
    }

    private static object? DynamicAccess(IElement element, string propName)
    {
        var type = element.GetType();
        var typeProp = type.GetProperty(propName) ??
                       throw new ArgumentOutOfRangeException(
                           nameof(propName),
                           $"{type} does not have '{propName}' property.");
        return typeProp.GetValue(element);
    }
}