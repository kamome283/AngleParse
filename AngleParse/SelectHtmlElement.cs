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
    public object[] Selector { get; set; } = null!;

    [Parameter(
        Position = 1,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true,
        HelpMessage = "HTML content.")]
    public string Content { get; set; } = null!;

    private ISelector<ElementResource, ObjectResource> Pipeline { get; set; } = null!;
    private ElementResource ElementResource { get; set; } = null!;

    protected override void BeginProcessing()
    {
        Pipeline = SelectorFactory.CreatePipeline(Selector);
        ElementResource = ElementResource.CreateAsync(Content).GetAwaiter().GetResult();
    }

    protected override void ProcessRecord()
    {
        WriteObject(
            Pipeline
                .Select(ElementResource)
                .Select(r => new PSObject(r.Object))
                .ToArray(),
            true
        );
    }
}