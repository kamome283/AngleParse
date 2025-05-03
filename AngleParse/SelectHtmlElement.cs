using System;
using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;
using AngleParse.Selector;

namespace AngleParse;

[Cmdlet(VerbsCommon.Select, "HtmlContent", HelpUri = "https://github.com/kamome283/AngleParse")]
[OutputType(typeof(PSObject))]
public class SelectHtmlElement : PSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        HelpMessage = "Selector to select and process data in the content.")]
    public object[]? Selector { get; set; }

    [Parameter(
        Position = 1,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "HTML content.")]
    public string? Content { get; set; }

    private ISelector<ElementResource, ObjectResource> Pipeline { get; set; } = null!;

    protected override void BeginProcessing()
    {
        if (Selector is null || Selector.Length == 0)
            throw new ArgumentNullException(nameof(Selector), "Selector cannot be null or empty.");
        Pipeline = SelectorFactory.CreatePipeline(Selector);
    }

    protected override void ProcessRecord()
    {
        if (Content is null)
            throw new ArgumentNullException(nameof(Content), "Content cannot be null.");
        var elementResource = ElementResource
            .CreateAsync(Content)
            .GetAwaiter()
            .GetResult();
        WriteObject(
            Pipeline
                .Select(elementResource)
                .Select(r => new PSObject(r.Object))
                .ToArray(),
            true
        );
    }
}