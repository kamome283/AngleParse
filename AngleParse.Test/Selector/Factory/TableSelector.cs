using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector.Factory;

public class TableSelector
{
    private static string ValidCssSelectorExpr => "div > p";
    private static string InvalidCssSelectorExpr => "div > p >";
    private static Regex TestingRegex => new("tag: (.*)");
    private static ScriptBlock TestingScriptBlock => ScriptBlock.Create("{ $_.Name -eq 'test' }");

    [Fact]
    public void CanCreateWithOneElementSelector()
    {
        var tableSelector = GetTableSelector(
            ("cssSelector", ValidCssSelectorExpr));
        Assert.IsType<TableSelector<ElementResource>>(tableSelector);
    }

    [Fact]
    public void CanCreateWithOneStringSelector()
    {
        var tableSelector = GetTableSelector(
            ("regex", TestingRegex));
        Assert.IsType<TableSelector<StringResource>>(tableSelector);
    }

    [Fact]
    public void CanCreateWithOneObjectSelector()
    {
        var tableSelector = GetTableSelector(
            ("scriptBlock", TestingScriptBlock));
        Assert.IsType<TableSelector<ObjectResource>>(tableSelector);
    }

    [Fact]
    public void CanCreateWithMultipleSelectors()
    {
        var tableSelector = GetTableSelector(
            ("regex", TestingRegex),
            ("scriptBlock", TestingScriptBlock));
        Assert.IsType<TableSelector<StringResource>>(tableSelector);
    }

    [Fact]
    public void CreateWithInvalidSelectorWillThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GetTableSelector(
                ("scriptBlock", TestingScriptBlock),
                ("invalid", InvalidCssSelectorExpr),
                ("cssSelector", ValidCssSelectorExpr));
        });
    }

    private static dynamic GetTableSelector(params (object key, object? value)[] pairs)
    {
        var hashtable = new Hashtable(pairs.ToDictionary());
        return SelectorFactory.CreateSelector(hashtable);
    }
}