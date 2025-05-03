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
    Context 'AttributeSelector' {
        Context 'Truly attribute' {
            It 'selects the attribute value' {
                $attr = [AngleParse.Attr]'some-attribute'
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent $attr
                $result | should -be 'some_value'
            }
            It 'outputs empty string if the attribute does not have value' {
                $attr = [AngleParse.Attr]'valueless-attribute'
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent $attr
                $result | should -be ''
            }
            It 'outputs null if the attribute does not exist' {
                $attr = [AngleParse.Attr]'nonexistent-attribute'
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent $attr
                $result | should -BeNullOrEmpty
            }
            It 'class-defined attributes works as well' -ForEach @(
                @{ Attr = [AngleParse.Attr]::Href; Expected = 'https://some_url_in_japan.go.jp' }
                @{ Attr = [AngleParse.Attr]::Src; Expected = 'https://some_url_in_japan.go.jp/some_pic.jpg' }
                @{ Attr = [AngleParse.Attr]::Title; Expected = 'Some title' }
                @{ Attr = [AngleParse.Attr]::Name; Expected = 'some_name' }
            ) {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent $attr
                $result | should -be $Expected
            }
        }
        Context 'Not truly attribute' {
            It 'InnerHtml works' {
                # If not enclosed in parentheses, it will be evaluated as a string instead of an Attr.
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::InnerHtml)
                $result | should -be '<span>some link</span>'
            }
            It 'OuterHtml works' {
                $result = Get-Asset 'no-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::OuterHtml)
                $result | should -be '<a><span>some link</span></a>'
            }
            It 'TextContent works' {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::TextContent)
                $result | should -be 'some link'
            }
            It 'Id works' {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::Id)
                $result | should -be 'some_id'
            }
            It 'ClassName works' {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::ClassName)
                $result | should -be 'some_class another_class'
            }
            It 'SplitClasses works' {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::SplitClasses)
                $result | should -be 'some_class', 'another_class'
            }
        }
    }
    Context 'CssSelector' {
        It 'throws if the selector is not valid' {
            {
                Get-Asset 'full.html' | Select-HtmlContent 'div > p >'
            } | should -throw
        }
        It 'selects the element by CSS selector' {
            $result = Get-Asset 'full.html' | Select-HtmlContent 'section#fragment > div.some_class'
            $result.Length | should -be 2
        }
        It 'returns null if the selector does not match anything' {
            $result = Get-Asset 'full.html' | Select-HtmlContent 'section#fragment > div.nonexistent_class'
            $result | should -be $null
        }
        It 'returns null if the document is empty' {
            $result = Get-Asset 'empty.html' | Select-HtmlContent 'div'
            $result | should -be $null
        }
    }
    Context 'PropertySelector' {
        It 'retunrs inner IElement when the selector is Prop.Element' {
            $result = Get-Asset 'full.html' | Select-HtmlContent ([AngleParse.Prop]::Element)
            $result | should -beOfType AngleSharp.Dom.IElement
        }
        It 'returns attribute table when the selector is Prop.AttributesTable' {
            $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]::AttributesTable)
            $result.class | should -be 'some_class another_class'
            $result.href | should -be 'https://some_url_in_japan.go.jp'
            $result.id | should -be 'some_id'
            $result.name | should -be 'some_name'
            $result.'some-attribute' | should -be 'some_value'
            $result.src | should -be 'https://some_url_in_japan.go.jp/some_pic.jpg'
            $result.title | should -be 'Some title'
            $result.'valueless-attribute' | should -be ''
            $result.nonexistent_attribute | should -be $null
        }
        It 'retunrs inner IElement property value with undefined property selector' {
            $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]'ClassName')
            $result | should -be 'some_class another_class'
        }
        It 'throws if the property does not exist' {
            {
                Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]'NonexistentProperty')
            } | should -throw
        }
    }
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
            $result | should -be $null
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