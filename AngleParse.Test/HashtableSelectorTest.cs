using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Selector;
using Xunit;

namespace AngleParse.Test
{
    public class HashtableSelectorTest
    {
        private readonly ElementResource resource = new ValidElementResource();

        private readonly HashtableSelector selector = new ValidHashtableSelector();

        [Fact]
        public void InitializingFromHashtableWorks()
        {
            var ht = new Hashtable {{"class", Attr.Class}};
            var _ = new HashtableSelector(ht);
        }

        [Fact]
        public void SelectorWorks()
        {
            var actual = selector.Select(resource);
            Assert.Single(actual);
            var first = actual.First().AsObject();
            var d = first as Dictionary<object, object[]>;
            Assert.NotNull(d);

            var redirectLinks = d["redirectLinks"];
            var redirectLinksExpected = new object[]
                {"Windows_XP_SP2", "Windows_Server_2003_SP1", "General_availability"};
            Assert.Equal(redirectLinksExpected, redirectLinks);

            var cls = d["class"];
            Assert.Empty(cls);
        }
    }
}