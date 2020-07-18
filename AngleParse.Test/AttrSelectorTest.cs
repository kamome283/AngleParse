using System;
using System.Linq;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleParse.Test.Resource.AttrElementResource;
using AngleParse.Test.Resource.ElementResource;
using AngleParse.Test.Resource.StringResource;
using Xunit;

namespace AngleParse.Test
{
    public class AttrSelectorTest
    {
        private static readonly ElementResource validResource = new ValidAttrElementResource();
        private static readonly ElementResource emptyResource = new EmptyAttrElementResource();
        private static readonly ElementResource bodyResource = new ValidElementResource();
        private static readonly StringResource stringResource = new ValidStringResource();

        private static void AttrTest(Attr attr, IResource resource, params string[] expected)
        {
            var selector = new AttrSelector(attr);
            var actual = selector.Select(resource).Select(r => r.AsString());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InitializingSelectorWithInvalidAttrThrowsTypeInitializationException()
        {
            Assert.Throws<TypeInitializationException>(() =>
            {
                // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
                var _ = new AttrSelector((Attr) Int32.MaxValue);
            });
        }

        [Fact]
        public void SelectClassWorks()
        {
            AttrTest(Attr.Class, validResource, "some_class another_class");
        }

        [Fact]
        public void SelectHrefWorks()
        {
            AttrTest(Attr.Href, validResource, "https://some_url_in_japan.go.jp");
        }

        [Fact]
        public void SelectIdWorks()
        {
            AttrTest(Attr.Id, validResource, "some_id");
        }

        [Fact]
        public void SelectInnerHtmlWorks()
        {
            AttrTest(Attr.InnerHtml, emptyResource, @"Some text and <a>some link</a>.");
        }

        [Fact]
        public void SelectNameWorks()
        {
            AttrTest(Attr.Name, validResource, "some_name");
        }

        [Fact]
        public void SelectNonExistAttributesReturnsEmptySequence()
        {
            foreach (var attr in Enum.GetValues(typeof(Attr)).Cast<Attr>()
                .Except(new[] {Attr.InnerHtml, Attr.OuterHtml, Attr.TextContent}))
            {
                AttrTest(attr, emptyResource);
            }
        }

        [Fact]
        public void SelectOnBodyElementAttributesReturnsEmptySequence()
        {
            foreach (var attr in Enum.GetValues(typeof(Attr)).Cast<Attr>()
                .Except(new[] {Attr.InnerHtml, Attr.OuterHtml, Attr.TextContent}))
            {
                AttrTest(attr, bodyResource);
            }
        }

        [Fact]
        public void SelectOnStringResourceThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => { AttrTest(Attr.Id, stringResource); });
        }

        [Fact]
        public void SelectOuterHtmlWorks()
        {
            AttrTest(Attr.OuterHtml, emptyResource, @"<p>Some text and <a>some link</a>.</p>");
        }

        [Fact]
        public void SelectSplitClassesWorks()
        {
            AttrTest(Attr.SplitClasses, validResource, "some_class", "another_class");
        }

        [Fact]
        public void SelectSrcWorks()
        {
            AttrTest(Attr.Src, validResource, "https://some_url_in_japan.go.jp/some_pic.jpg");
        }

        [Fact]
        public void SelectTextContentWorks()
        {
            AttrTest(Attr.TextContent, emptyResource, "Some text and some link.");
        }

        [Fact]
        public void SelectTitleWorks()
        {
            AttrTest(Attr.Title, validResource, "Some title");
        }
    }
}