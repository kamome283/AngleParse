using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using AngleParse.Resource;
using AngleParse.Selector;
using AngleSharp;
using AngleSharp.Dom;

namespace AngleParse;

[Cmdlet(VerbsCommon.Select, "HtmlContent", HelpUri = "https://github.com/kamome283/AngleParse")]
[OutputType(typeof(PSObject[]))]
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
    private IElement Element { get; set; } = null!;

    protected override void BeginProcessing()
    {
        Pipeline = SelectorFactory.CreateSelector(Selector);
        Element = GetElementAsync(Content).GetAwaiter().GetResult();
    }

    protected override void ProcessRecord()
    {
        var resource = new ElementResource(Element);
        WriteObject(
            Pipeline.Select(resource).Select(r => new PSObject(r.Object)).ToArray(),
            true
        );
    }

    private static async Task<IElement> GetElementAsync(string Content)
    {
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var doc = await context.OpenAsync(res => res.Content(Content));
        if (doc.Body is null) throw new ArgumentOutOfRangeException(nameof(Content));
        return doc.Body.ChildElementCount == 1 ? doc.Body.Children.First() : doc.Body;
    }
}