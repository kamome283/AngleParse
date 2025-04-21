// ReSharper disable StringLiteralTypo

namespace AngleParse.Test.Resource.ElementResource;

public class ValidElementResource : AngleParse.Resource.ElementResource
{
    private const string Body = @"
<div class=""some_class"">
    <h3><span class=""mw-headline"" id=""Windows_PowerShell_1.0"">Windows PowerShell 1.0</span></h3>
    <p>
        PowerShell 1.0 was released in November 2006 for 
        <a href=""/wiki/Windows_XP_SP2"" class=""mw-redirect"" title=""Windows XP SP2"">Windows XP SP2</a>,
        <a href=""/wiki/Windows_Server_2003_SP1"" class=""mw-redirect"" title=""Windows Server 2003 SP1"">Windows Server 2003 SP1</a> and 
        <a href=""/wiki/Windows_Vista"" title=""Windows Vista"">Windows Vista</a>.
        <sup id=""cite_ref-58"" class=""reference""><a href=""#cite_note-58"">[58]</a></sup>
        It is an optional component of <a href=""/wiki/Windows_Server_2008"" title=""Windows Server 2008"">Windows Server 2008</a>.
    </p>
</div>
<div class=""some_class"">
    <h3>
        <span class=""mw-headline"" id=""PowerShell_Core_6"">PowerShell Core 6</span>
        <span class=""mw-editsection"">
            <span class=""mw-editsection-bracket"">[</span>
            <a href=""/w/index.php?title=PowerShell&amp;action=edit&amp;section=15"" title=""Edit section: PowerShell Core 6"">edit</a>
            <span class=""mw-editsection-bracket"">]</span>
        </span>
    </h3>
    <p>
        PowerShell Core 6.0 was first announced on 18 August 2016, when Microsoft unveiled PowerShell Core and its decision to make the product cross-platform,
        independent of Windows, free and open source.
        <sup id=""cite_ref-ars-opensource_5-2"" class=""reference""><a href=""#cite_note-ars-opensource-5"">[5]</a></sup>
        It achieved <a href=""/wiki/General_availability"" class=""mw-redirect"" title=""General availability"">general availability</a> 
        on 10 January 2018 for Windows, macOS and Linux.
        <sup id=""cite_ref-89"" class=""reference""><a href=""#cite_note-89"">[89]</a></sup>
        It has its own support lifecycle and adheres to the Microsoft lifecycle policy that is introduced with Windows 10:
        Only the latest version of PowerShell Core is supported.
        Microsoft expects to release one minor version for PowerShell Core 6.0 every six months.
        <sup id=""cite_ref-90"" class=""reference""><a href=""#cite_note-90"">[90]</a></sup>
    </p>
</div>
";

    public ValidElementResource() : base(Body)
    {
    }
}