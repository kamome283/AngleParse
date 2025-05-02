using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public sealed class PropertySelectorTests
{
    [Fact]
    public void Create()
    {
        var prop = new Prop("some-property");
        var propertySelector = SelectorFactory.CreateSelector(prop);
        Assert.IsType<PropertySelector>(propertySelector);
    }
}