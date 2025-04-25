using System;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;

namespace AngleParse.Selector;

internal static class SelectorFactory
{
    public static dynamic CreateSelector(object? obj) => obj switch
    {
        string cssSelectorExpr => new CssSelector(cssSelectorExpr),
        Regex regex => new RegexSelector(regex),
        Attr attribute => new AttributeSelector(attribute),
        ScriptBlock scriptBlock => new ScriptBlockSelector(scriptBlock),
        //TODO: Hashtable hashtable => CreateTableSelector(hashtable),
        object[] objects => CreateFuncSelector(objects),
        _ => throw new ArgumentOutOfRangeException(nameof(obj), obj, "Invalid selector type")
    };

    private static dynamic CreateFuncSelector(object[] objects)
    {
        dynamic selector = FuncSelector<ElementResource, ElementResource>.Identity;
        foreach (var obj in objects)
        {
            var nextSelector = CreateSelector(obj);
            // If the selector can connect to the next selector then connect them,
            // otherwise throw an exception
            selector = Connect(selector, nextSelector);
        }

        return selector;
    }

    private static FuncSelector<LIn, ROut> Connect<LIn, LOut, RIn, ROut>(
        ISelector<LIn, LOut> left, ISelector<RIn, ROut> right)
        where LIn : LOut
        where LOut : RIn
        where RIn : ROut
        where ROut : ObjectResource =>
        new(x => left.Select(x).SelectMany(right.Select));
}