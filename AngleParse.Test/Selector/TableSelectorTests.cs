using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleParse.Resource;
using AngleParse.Selector;
using Xunit;
using static AngleParse.Test.Helpers.ElementResourceFactory;

namespace AngleParse.Test.Selector;

public sealed class CreationTests
{
    private static string ValidCssSelectorExpr => "div > p";
    private static string InvalidCssSelectorExpr => "div > p >";
    private static Regex ValidRegex => new("tag: (.*)");
    private static ScriptBlock ValidScriptBlock => ScriptBlock.Create("{ $_.Name -eq 'test' }");

    [Fact]
    public void CreateWithOneElementSelector()
    {
        var tableSelector = SelectorFactory.CreateSelector(new Hashtable
        {
            { "cssSelector", ValidCssSelectorExpr }
        });
        Assert.IsType<TableSelector<ElementResource>>(tableSelector);
    }

    [Fact]
    public void CreateWithOneStringSelector()
    {
        var tableSelector = SelectorFactory.CreateSelector(new Hashtable
        {
            { "regex", ValidRegex }
        });
        Assert.IsType<TableSelector<StringResource>>(tableSelector);
    }

    [Fact]
    public void CreateWithOneObjectSelector()
    {
        var tableSelector = SelectorFactory.CreateSelector(new Hashtable
        {
            { "scriptBlock", ValidScriptBlock }
        });
        Assert.IsType<TableSelector<ObjectResource>>(tableSelector);
    }

    [Fact]
    public void CreateWithMultipleSelectors()
    {
        var tableSelector = SelectorFactory.CreateSelector(new Hashtable
        {
            { "regex", ValidRegex },
            { "scriptBlock", ValidScriptBlock }
        });
        Assert.IsType<TableSelector<StringResource>>(tableSelector);
    }

    [Fact]
    public void CreatingWithInvalidExpressionThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            SelectorFactory.CreateSelector(new Hashtable
            {
                { "cssSelector", InvalidCssSelectorExpr },
                { "invalidSelector", "invalid" },
                { "scriptBlock", ValidScriptBlock },
                { "regex", ValidRegex }
            });
        });
    }
}

public class SelectionTests
{
    [Fact]
    public async Task EachCaseMatchesOneElement()
    {
        var pipeline = SelectorFactory.CreatePipeline(new object?[]
        {
            "div.some_class:nth-child(1)",
            "span.mw-headline",
            new Hashtable
            {
                { "class", Attr.Class },
                { "text", Attr.TextContent }
            }
        });
        var resource = await CreateElementResourceAsync("full.html");
        var expected = new Hashtable
        {
            { "class", "mw-headline" },
            { "text", "Windows PowerShell 1.0" }
        };
        Assert.Single(pipeline.Select(resource), item =>
        {
            var result = (PSObject)item.Object;
            Assert.Equal(expected, result.BaseObject);
            return true;
        });
    }

    [Fact]
    public async Task EachCaseMatchesMultipleElements()
    {
        var pipeline = SelectorFactory.CreatePipeline(new object?[]
        {
            "div.some_class:nth-child(1)",
            new Hashtable
            {
                { "redirect", new object[] { "a.mw-redirect", Attr.TextContent } }
            }
        });
        var expected = new Hashtable
        {
            { "redirect", new object?[] { "Windows XP SP2", "Windows Server 2003 SP1" } }
        };
        var resource = await CreateElementResourceAsync("full.html");
        Assert.Single(pipeline.Select(resource), item =>
        {
            var result = (PSObject)item.Object;
            Assert.Equal(expected, result.BaseObject);
            return true;
        });
    }

    [Fact]
    public async Task EachCaseMatchesNoElements()
    {
        var pipeline = SelectorFactory.CreatePipeline(new object?[]
        {
            "div.some_class:nth-child(1)",
            new Hashtable
            {
                {
                    "redirect", new object[]
                    {
                        "a.mw-redirect",
                        Attr.TextContent,
                        new Regex("not-matching-pattern: (.+)")
                    }
                }
            }
        });
        var expected = new Hashtable
        {
            { "redirect", null }
        };
        var resource = await CreateElementResourceAsync("full.html");
        Assert.Single(pipeline.Select(resource), item =>
        {
            var result = (PSObject)item.Object;
            Assert.Equal(expected, result.BaseObject);
            return true;
        });
    }

    [Fact]
    public async Task ComplexSelector()
    {
        var pipeline = SelectorFactory.CreatePipeline(new object?[]
        {
            "div.some_class",
            new Hashtable
            {
                {
                    "windows", new object[]
                    {
                        "a.mw-redirect",
                        Attr.TextContent,
                        new Regex("Windows( .+)"),
                        new Regex("""(\S+)""")
                    }
                },
                { "title", new object[] { "a.mw-redirect", Attr.Title } }
            }
        });
        var resource = await CreateElementResourceAsync("full.html");
        var expected = new[]
        {
            new Hashtable
            {
                { "windows", new object[] { "XP", "SP2", "Server", "2003", "SP1" } },
                { "title", new object[] { "Windows XP SP2", "Windows Server 2003 SP1" } }
            },
            new Hashtable
            {
                { "windows", null },
                { "title", "General availability" }
            }
        };
        var result = pipeline.Select(resource).Select(r =>
            (Hashtable)((PSObject)r.Object).BaseObject);
        Assert.Equal(expected, result);
    }
}