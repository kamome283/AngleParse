using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource;
using AngleParse.Test.Selector;
using AngleSharp.Dom;
using Xunit;

namespace AngleParse.Test
{
    public class StringSelectorTest
    {
        private readonly StringSelector validSelector = new ValidStringSelector();
        private readonly StringSelector invalidSelector = new InvalidStringSelector();
        private readonly StringSelector notMatchingSelector = new NotMatchingStringSelector();

        private readonly ElementResource validElementResource = new ValidElementResource();
        private readonly ElementResource invalidElementResource = new InvalidElementResource();

        [Fact]
        public void SelectByInvalidSelectorThrowsException()
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
        public void SelectOnInvalidResourceByInvalidSelectorThrowsException()
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
        public void ValidSelectorOnValidResourceReturnsExpectedValues()
        {
            var expected = new[] {"Windows XP SP2", "Windows Server 2003 SP1", "general availability"};
            var actual = validSelector.Select(validElementResource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }
    }
}