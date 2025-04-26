using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using Microsoft.CSharp.RuntimeBinder;

namespace AngleParse.Selector;

internal static class SelectorFactory
{
    public static ISelector<ElementResource, ObjectResource> CreatePipeline(object? obj) =>
        CreateSelector(obj);

    private static dynamic CreateSelector(object? obj) => obj switch
    {
        string cssSelectorExpr => new CssSelector(cssSelectorExpr),
        Regex regex => new RegexSelector(regex),
        Attr attribute => new AttributeSelector(attribute),
        ScriptBlock scriptBlock => new ScriptBlockSelector(scriptBlock),
        Hashtable hashtable => throw new NotImplementedException(),
        object[] objects => CreateFuncSelector(objects),
        null => throw new ArgumentNullException(nameof(obj)),
        _ => throw new ArgumentOutOfRangeException(nameof(obj), obj,
            $"Invalid selector type: {obj.GetType()}")
    };

    private static dynamic CreateFuncSelector(object[] objects)
    {
        dynamic selector = FuncSelector<ElementResource, ElementResource>.Identity;
        foreach (var obj in objects)
        {
            var nextSelector = CreateSelector(obj);
            try
            {
                selector = Connect(selector, nextSelector);
            }
            catch (RuntimeBinderException e)
            {
                throw new InvalidOperationException(
                    $"Cannot connect {selector.GetType()} to {nextSelector.GetType()}", e);
            }
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