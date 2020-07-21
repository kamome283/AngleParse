using System.Linq;
using System.Management.Automation;
using AngleParse.Resource;
using AngleParse.Selector;

namespace AngleParse
{
    [Cmdlet(VerbsCommon.Select, "HtmlContent", HelpUri = "https://github.com/kamome283/AngleParse")]
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

        [Parameter(HelpMessage = @"
Allow DOM manipulation by content internal scripts.")]
        public bool DomManipulation { get; set; } = true;

        private ISelector InSelector { get; set; }

        protected override void BeginProcessing()
        {
            InSelector = new PipelineSelector(Selector);
        }

        protected override void ProcessRecord()
        {
            var resource = new ElementResource(Content, DomManipulation);
            WriteObject(
                InSelector.Select(resource).Select(r => r.AsObject()),
                true
            );
        }
    }
}