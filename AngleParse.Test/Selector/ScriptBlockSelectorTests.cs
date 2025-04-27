using System.Management.Automation;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public class ScriptBlockSelectorTests
{
    [Fact]
    public void Create()
    {
        var scriptBlock = ScriptBlock.Create("{ $_.Name -eq 'test' }");
        var scriptBlockSelector = SelectorFactory.CreateSelector(scriptBlock);
        Assert.IsType<ScriptBlockSelector>(scriptBlockSelector);
    }
}