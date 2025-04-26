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

    private static dynamic CreateSelector(object? obj) => obj switch
    {
        string cssSelectorExpr => new CssSelector(cssSelectorExpr),
        Regex regex => new RegexSelector(regex),
        Attr attribute => new AttributeSelector(attribute),
        ScriptBlock scriptBlock => new ScriptBlockSelector(scriptBlock),
        Hashtable hashtable => CreateTableSelector(hashtable),
        object[] objects => CreateFuncSelector(objects),
        null => throw new ArgumentNullException(nameof(obj)),
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

        dynamic dynamicSelectors = selectors;

        return dynamicSelectors switch
        {
            // Since In of ISelector is contravariant,
            // ObjectResource is the most specific type that can appear in this position.
            IReadOnlyList<ISelector<ObjectResource, ObjectResource>> objectSelectors =>
                new TableSelector<ObjectResource>(CreateDictionary(keys, objectSelectors)),
            IReadOnlyList<ISelector<StringResource, ObjectResource>> stringSelectors =>
                new TableSelector<StringResource>(CreateDictionary(keys, stringSelectors)),
            IReadOnlyList<ISelector<ElementResource, ObjectResource>> elementSelectors =>
                new TableSelector<ElementResource>(CreateDictionary(keys, elementSelectors)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(hashtable),
                $"Cannot create table selector from {hashtable}")
        };
    }

    private static Dictionary<object, ISelector<In, ObjectResource>> CreateDictionary<In>(
        IReadOnlyList<object> keys, IReadOnlyList<ISelector<In, ObjectResource>> selectors)
        where In : ObjectResource
    {
        if (keys.Count != selectors.Count)
            throw new InvalidOperationException("Keys and selectors count mismatch.");
        return keys.Zip(selectors).ToDictionary();
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