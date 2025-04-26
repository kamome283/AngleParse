using System;
using System.Collections;
using System.Collections.Generic;
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

    internal static dynamic CreateSelector(object? obj) => obj switch
    {
        string cssSelectorExpr => new CssSelector(cssSelectorExpr),
        Regex regex => new RegexSelector(regex),
        Attr attribute => new AttributeSelector(attribute),
        ScriptBlock scriptBlock => new ScriptBlockSelector(scriptBlock),
        Hashtable hashtable => CreateTableSelector(hashtable),
        object[] objects => CreateFuncSelector(objects),
        null => throw new ArgumentOutOfRangeException(nameof(obj), "null cannot be a selector"),
        _ => throw new ArgumentOutOfRangeException(nameof(obj), obj,
            $"Invalid selector type: {obj.GetType()}")
    };

    private static dynamic CreateTableSelector(Hashtable hashtable)
    {
        // Since Dictionary does not support variance, we need to make it from lists.
        List<object> keys = [];
        List<dynamic> selectors = [];
        foreach (var entry in hashtable.Cast<DictionaryEntry>())
        {
            keys.Add(entry.Key);
            selectors.Add(CreateSelector(entry.Value));
        }

        List<T>? CastList<T>(List<dynamic> list)
        {
            var casted = list.OfType<T>().ToList();
            return casted.Count == list.Count ? casted : null;
        }

        Dictionary<T, R> GetDictionary<T, R>(List<T> ks, List<R> vs) where T : notnull
        {
            if (ks.Count != vs.Count)
                throw new InvalidOperationException("Keys and selectors count mismatch.");
            return ks.Zip(vs).ToDictionary();
        }

        // Since In of ISelector is contravariant,
        // ObjectResource is the most specific type that can appear in this position.
        var objectSelectors = CastList<ISelector<ObjectResource, ObjectResource>>(selectors);
        if (objectSelectors is not null)
            return new TableSelector<ObjectResource>(GetDictionary(keys, objectSelectors));
        var stringSelectors = CastList<ISelector<StringResource, ObjectResource>>(selectors);
        if (stringSelectors is not null)
            return new TableSelector<StringResource>(GetDictionary(keys, stringSelectors));
        var elementSelectors = CastList<ISelector<ElementResource, ObjectResource>>(selectors);
        if (elementSelectors is not null)
            return new TableSelector<ElementResource>(GetDictionary(keys, elementSelectors));
        throw new ArgumentOutOfRangeException(
            nameof(selectors),
            $"Cannot create table selector from {selectors.GetType()}");
    }

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