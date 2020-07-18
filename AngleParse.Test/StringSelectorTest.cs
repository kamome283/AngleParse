using System;
using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.StringResource;
using AngleParse.Test.Selector.StringSelector;
using AngleSharp.Dom;
using Xunit;

namespace AngleParse.Test
{
    public class StringSelectorTest
    {
        private static readonly StringSelector validSelector = new ValidStringSelector();
        private static readonly StringSelector invalidSelector = new InvalidStringSelector();
        private static readonly StringSelector notMatchingSelector = new NotMatchingStringSelector();

        private static readonly ElementResource validElementResource = new ValidElementResource();
        private static readonly ElementResource invalidElementResource = new InvalidElementResource();
        private static readonly StringResource validStringResource = new ValidStringResource();

        [Fact]
        public void SelectByInvalidSelectorThrowsDomException()
        {
            Assert.Throws<DomException>(() => invalidSelector.Select(validElementResource));
        }

        [Fact]
        public void SelectByNotMatchingSelectorReturnsEmptySequence()
        {
            var expected = new string[] { };
            var actual = notMatchingSelector.Select(validElementResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectByValidSelectorWorks()
        {
            var expected = new[] {"Windows XP SP2", "Windows Server 2003 SP1", "general availability"};
            var actual = validSelector.Select(validElementResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectOnInvalidResourceByInvalidSelectorThrowsDomException()
        {
            Assert.Throws<DomException>(() => invalidSelector.Select(invalidElementResource));
        }

        [Fact]
        public void SelectOnInvalidResourceReturnsEmptySequence()
        {
            var expected = new string[] { };
            var actual = validSelector.Select(invalidElementResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SelectOnStringResourceThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => validSelector.Select(validStringResource));
        }
    }
}