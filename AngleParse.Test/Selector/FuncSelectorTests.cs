using System;
using System.Management.Automation;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public static class FuncSelectorTests
{
    public class CreationTests
    {
        [Fact]
        public void CreatingWithEmptyArrayThrowsException()
        {
            object[] empty = [];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                SelectorFactory.CreateSelector(empty);
            });
        }

        [Fact]
        public void CreatingWithOneObjectReturnsInternalSelector()
        {
            object[] objects = ["div > p"];
            var selector = SelectorFactory.CreateSelector(objects);
            Assert.IsType<CssSelector>(selector);
        }

        [Fact]
        public void CreateWithTwoObjects()
        {
            object[] objects =
            [
                new Regex("tag: (.*)"),
                ScriptBlock.Create("{ $_.Name -eq 'test' }")
            ];
            var selector = SelectorFactory.CreateSelector(objects);
            Assert.IsType<FuncSelector<StringResource, ObjectResource>>(selector);
        }

        [Fact]
        public void CreatingPipelineWithOneObjectCastsProperly()
        {
            object[] objects = [new Regex("tag: (.*)")];
            var selector = SelectorFactory.CreatePipeline(objects);
            Assert.IsType<ISelector<ElementResource, ObjectResource>>(selector, false);
        }

        [Fact]
        public void CreatingPipelineWithTwoObjectsCastsProperly()
        {
            object[] objects =
            [
                new Regex("tag: (.*)"),
                new Regex("(.*),")
            ];
            var selector = SelectorFactory.CreatePipeline(objects);
            Assert.IsType<ISelector<ElementResource, ObjectResource>>(selector, false);
        }

        [Fact]
        public void CreatingUnconnectablePipelineThrowsException()
        {
            object[] objects =
            [
                new Regex("tag: (.*)"),
                Attr.Href
            ];
            Assert.Throws<InvalidOperationException>(() =>
            {
                SelectorFactory.CreatePipeline(objects);
            });
        }

        [Fact]
        public void CreatingWithInvalidTypeObjectThrowsException()
        {
            object[] objects =
            [
                new Regex("tag: (.*)"),
                0
            ];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                SelectorFactory.CreateSelector(objects);
            });
        }

        [Fact]
        public void CreatingWithInvalidExpressionThrowsException()
        {
            object[] objects =
            [
                "div > p",
                "div > p >"
            ];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                SelectorFactory.CreateSelector(objects);
            });
        }
    }
}