using System.Management.Automation;
using AngleParse.Selector;
using Xunit;

namespace AngleParse.Test.Selector;

public sealed class ScriptBlockSelectorTests
{
    [Fact]
    public void Create()
    {
        var scriptBlock = ScriptBlock.Create("{ $_.Name -eq 'test' }");
        var scriptBlockSelector = SelectorFactory.CreateSelector(scriptBlock);
        Assert.IsType<ScriptBlockSelector>(scriptBlockSelector);
    }

    // ScriptBlock invocation from C# proved unreliable (e.g., runspace context issues).
    // Tests about selection are implemented using Pester instead.
}