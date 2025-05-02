using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleParse.Selector;
using Xunit;
using static AngleParse.Test.Helpers.ElementResourceFactory;

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

    [Fact]
    public async Task SelectByElementProp()
    {
        var prop = Prop.Element;
        PropertySelector selector = SelectorFactory.CreateSelector(prop);
        var resource = await CreateElementResourceAsync("full-attribute.html");
        Assert.Single(selector.Select(resource).Select(r => r.Object), resource.Element);
    }

    [Fact]
    public async Task SelectByAttributesTableProp()
    {
        var prop = Prop.AttributesTable;
        PropertySelector selector = SelectorFactory.CreateSelector(prop);
        var resource = await CreateElementResourceAsync("full-attribute.html");
        var selected = selector
            .Select(resource)
            .Select(r => (Dictionary<string, string>)r.Object)
            .ToList();
        var expected = new Dictionary<string, string>
        {
            ["class"] = "some_class another_class",
            ["href"] = "https://some_url_in_japan.go.jp",
            ["id"] = "some_id",
            ["name"] = "some_name",
            ["some-attribute"] = "some_value",
            ["src"] = "https://some_url_in_japan.go.jp/some_pic.jpg",
            ["title"] = "Some title",
            ["valueless-attribute"] = ""
        };
        Assert.Single(selected);
        Assert.Equal(selected.First(), expected);
    }

    [Fact]
    public async Task SelectByUndefinedProperty()
    {
        var prop = new Prop("Text");
        PropertySelector selector = SelectorFactory.CreateSelector(prop);
        var resource = await CreateElementResourceAsync("full-attribute.html");
        Assert.Single(
            selector.Select(resource).Select(r => r.Object as string),
            item => item is not null && item == "some link");
    }

    [Fact]
    public async Task SelectingByNonExistingPropertyThrowsException()
    {
        var prop = new Prop("non-existing-property");
        PropertySelector selector = SelectorFactory.CreateSelector(prop);
        var resource = await CreateElementResourceAsync("full-attribute.html");
        Assert.Throws<ArgumentOutOfRangeException>(() => { selector.Select(resource); });
    }
}