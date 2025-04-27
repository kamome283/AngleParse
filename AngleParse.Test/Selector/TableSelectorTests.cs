using System;
using System.Collections;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public static class TableSelectorTests
{
    public class CreationTests
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
}