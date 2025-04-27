using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public class AttributeSelectorTests
{
    [Fact]
    public void Create()
    {
        const Attr attribute = Attr.Href;
        var attributeSelector = SelectorFactory.CreateSelector(attribute);
        Assert.IsType<AttributeSelector>(attributeSelector);
    }
}