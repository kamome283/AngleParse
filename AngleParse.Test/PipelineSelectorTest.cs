using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.StringResource;
using AngleParse.Test.Selector;
using Xunit;

namespace AngleParse.Test
{
    public class PipelineSelectorTest
    {
        private static readonly PipelineSelector validSelector =
            new PipelineSelector(
                new StringSelector("p > a.mw-redirect"),
                new AttrSelector(Attr.Href),
                new RegexSelector(new Regex("/wiki/(\\w+)"))
            );

        private static readonly PipelineSelector invalidSelector =
            new PipelineSelector(
                new StringSelector("p > a.mw-redirect"),
                new AttrSelector(Attr.Href),
                new StringSelector("*")
            );

        private static readonly PipelineSelector includingHashtableSelector =
            new PipelineSelector(
                new StringSelector("> div"),
                new ValidHashtableSelector()
            );

        private static readonly ElementResource validResource = new ValidElementResource();
        private static readonly ElementResource emptyResource = new EmptyElementResource();
        private static readonly StringResource stringResource = new ValidStringResource();

        [Fact]
        public void SelectByIncludingHashtableSelectorWorks()
        {
            var actual = includingHashtableSelector.Select(validResource);
            Assert.Equal(2, actual.Count());
            var first = actual.First().AsObject();
            var d = first as Dictionary<object, IEnumerable<object>>;
            Assert.NotNull(d);

            var redirectLinks = d["redirectLinks"];
            var redirectLinksExpected = new object[] {"Windows_XP_SP2", "Windows_Server_2003_SP1"};
            Assert.Equal(redirectLinksExpected, redirectLinks);

            var cls = d["class"];
            var clsExpected = new[] {"some_class"};
            Assert.Equal(cls, clsExpected);
        }

        [Fact]
        public void SelectByInvalidSelectorThrowsInvalidOperationException()
        {
            // To throw exception, you need to evaluate LINQ.
            Assert.Throws<InvalidOperationException>(() =>
                invalidSelector.Select(validResource).ToArray()
            );
        }

        [Fact]
        public void SelectOnEmptyResourceReturnsEmptySeq()
        {
            var actual = validSelector.Select(emptyResource).Select(r => r.AsString());
            Assert.Empty(actual);
        }


        [Fact]
        public void SelectOnStringResourceByElementRequiredPipelineThrowsInvalidOperationException()
        {
            // To throw exception, you need to evaluate LINQ.
            Assert.Throws<InvalidOperationException>(() =>
                validSelector.Select(stringResource).ToArray()
            );
        }

        [Fact]
        public void SelectOnValidResourceWorks()
        {
            var expected = new[] {"Windows_XP_SP2", "Windows_Server_2003_SP1", "General_availability"};
            var actual = validSelector.Select(validResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }
    }
}