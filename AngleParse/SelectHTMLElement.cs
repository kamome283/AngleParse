using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;
using AngleParse.Selector;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace AngleParse
{
    [Cmdlet(VerbsCommon.Select, "HtmlContent", HelpUri = "https://github.com/kamome283/AngleParse")]
    [OutputType(typeof(PSCustomObject))]
    public class SelectHtmlElement : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = @"
Selector to select and process data in the content.
For details, see https://github.com/kamome283/AngleParse
")]
        public object[] Selector { get; set; }

        [Parameter(
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = @"
HTML content.")]
        public string Content { get; set; }

        private ISelector InSelector { get; set; }

        protected override void BeginProcessing()
        {
            // See AngleParse.Selector.HashtableSelector.Select() for the reason to add casting script.
            var psCustomObjectCaster = ScriptBlock.Create("[pscustomobject] $_");
            InSelector = new PipelineSelector(Selector, psCustomObjectCaster);
        }

        protected override void ProcessRecord()
        {
            var resource = new ElementResource(Content);
            WriteObject(
                InSelector.Select(resource).Select(r => r.AsObject()),
                true
            );
        }
    }
}