using System;
using System.Collections.Generic;
using System.Linq;
using AngleParse.Resource;
using AngleSharp.Css.Parser;

namespace AngleParse.Selector;

internal sealed class CssSelector : ISelector<ElementResource, ElementResource>
{
    public CssSelector(string cssSelectorExpr)
    {
        if (Parser.ParseSelector(cssSelectorExpr) is null)
            throw new ArgumentOutOfRangeException(nameof(cssSelectorExpr));
        CssSelectorExpr = cssSelectorExpr;
    }

    private static CssSelectorParser Parser { get; } = new();

    private string CssSelectorExpr { get; }

    public IEnumerable<ElementResource> Select(ElementResource resource) =>
        resource
            .Element
            .QuerySelectorAll(CssSelectorExpr)
            .Select(e => new ElementResource(e));
}