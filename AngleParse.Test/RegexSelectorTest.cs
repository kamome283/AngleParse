using System;
using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.StringResource;
using AngleParse.Test.Selector.RegexSelector;
using Xunit;

namespace AngleParse.Test
{
    public class RegexSelectorTest
    {
        // Should throw exception
        // private static readonly RegexSelector invalidSelector = new InvalidRegexSelector();
        private static readonly RegexSelector validSelector = new ValidRegexSelector();
        private static readonly RegexSelector notMatchingSelector = new NotMatchingRegexSelector();
        private static readonly RegexSelector notCapturingSelector = new NotCapturingRegexSelector();

        private static readonly ElementResource validElementResource = new ValidElementResource();
        private static readonly ElementResource invalidElementResource = new InvalidElementResource();
        private static readonly StringResource validStringResource = new ValidStringResource();

        [Fact]
        public void InitializingInvalidSelectorThrowsTypeInitializationException()
        {
            Assert.Throws<TypeInitializationException>(() =>
            {
                var invalidSelector = new InvalidRegexSelector();
            });
        }

        [Fact]
        public void SelectByNotCapturingSelectorReturnsEmptySequence()
        {
            var expected = new string[] { };
            var actual = notCapturingSelector.Select(validStringResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectByNotMatchingSelectorReturnsEmptySequence()
        {
            var expected = new string[] { };
            var actual = notMatchingSelector.Select(validStringResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectOnInvalidElementResourceWorks()
        {
            var expected = new string[] { };
            var actual = validSelector.Select(invalidElementResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectOnValidElementResourceWorks()
        {
            var expected = new[]
            {
                "November", "2006", "Server", "2003", "Server", "2003", "Server", "2008", "Server", "2008", "August",
                "2016", "January", "2018"
            };
            var actual = validSelector.Select(validElementResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectOnValidStringResourceWorks()
        {
            var expected = new[]
            {
                "November", "2006", "Server", "2003", "Server", "2008", "Server", "2016", "Server", "2008", "Server",
                "2008", "Server", "2012", "Server", "2012",
            };
            var actual = validSelector.Select(validStringResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }
    }
}