BeforeAll {
    $solutionRoot = Split-Path -Parent $PSScriptRoot
    $implementationRoot = Join-Path $solutionRoot 'AngleParse'
    $projectFile = Join-Path $implementationRoot 'AngleParse.csproj'
    dotnet publish $projectFile
    if (Get-Module -Name AngleParse)
    {
        Remove-Module AngleParse
    }
    $moduleFile = Join-Path $implementationRoot 'bin/Release/AngleParse/AngleParse.psd1'
    Import-Module $moduleFile

    $assetsDir = Join-Path $solutionRoot 'AngleParse.Test/assets'
    $assetsCache = @{ }
    function Get-Asset([string]$filename)
    {
        $cache = $assetsCache[$filename]
        if ($cache)
        {
            return $cache
        }
        $file = Join-Path $assetsDir $filename
        if (Test-Path $file)
        {
            $content = Get-Content $file -Raw
            $assetsCache[$filename] = $content
            return $content
        }
        else
        {
            throw "File not found: $file"
        }
    }
}

Describe 'Select-HtmlContent' {
    Context 'ScriptBlockSelector' {
        It 'binds $_ in the scriptblock to the current element' {
            Get-Asset 'full-attribute.html' | Select-HtmlContent {
                $_ | should -be 'some link'
            }
        }
        It 'outputs the result of the scriptblock' {
            $result = Get-Asset 'full-attribute.html' | Select-HtmlContent {
                if ($_ -like 'some link')
                {
                    return 1
                }
            }
            $result | should -be 1
        }
        It 'outputs multiple results when the scriptblock returns multiple values' {
            $result = Get-Asset 'full-attribute.html' | Select-HtmlContent {
                if ($_ -like 'some link')
                {
                    return 1, 2
                }
            }
            $result | should -be 1, 2
        }
        It 'outputs null when the scriptblock returns nothing' {
            $result = Get-Asset 'full-attribute.html' | Select-HtmlContent { }
            $result | should -BeNullOrEmpty
        }
        It 'outputs the result even if the Write-Error is called in the scriptblock' {
            $result = Get-Asset 'full-attribute.html' | Select-HtmlContent {
                Write-Error -Message "Error in scriptblock" 2> $null
                return 1
            }
            $result | should -be 1
        }
        It 'throws if the scriptblock throws an exception' {
            {
                Get-Asset 'full-attribute.html' | Select-HtmlContent {
                    throw "Error in scriptblock"
                }
            } | should -throw
        }
    }
}