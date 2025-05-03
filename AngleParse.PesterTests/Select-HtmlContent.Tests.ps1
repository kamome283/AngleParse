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
            $cache[$filename] = $content
            return $content
        }
        else
        {
            throw "File not found: $file"
        }
    }
}
