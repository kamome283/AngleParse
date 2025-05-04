BeforeAll {
    $solutionRoot = Split-Path -Parent $PSScriptRoot

    if (Get-Module -Name AngleParse)
    {
        Remove-Module AngleParse
    }
    dotnet publish (Join-Path $solutionRoot 'AngleParse' 'AngleParse.csproj') -c Release
    Import-Module (Join-Path $solutionRoot 'AngleParse' 'bin/Release/AngleParse/AngleParse.psd1') -Force

    $assetsDir = Join-Path $solutionRoot 'AngleParse.Test' 'assets'
    $cacheTable = @{ }
    function Get-Asset([string]$filename)
    {
        $cache = $cacheTable[$filename]
        if ($cache)
        {
            return $cache
        }
        $file = Join-Path $assetsDir $filename
        if (Test-Path $file)
        {
            $asset = Get-Content $file -Raw
            $cacheTable[$filename] = $asset
            return $asset
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
                @{ AttrSb = { [AngleParse.Attr]::Href }; Expected = 'https://some_url_in_japan.go.jp' }
                @{ AttrSb = { [AngleParse.Attr]::Src }; Expected = 'https://some_url_in_japan.go.jp/some_pic.jpg' }
                @{ AttrSb = { [AngleParse.Attr]::Title }; Expected = 'Some title' }
                @{ AttrSb = { [AngleParse.Attr]::Name }; Expected = 'some_name' }
            ) {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent (& $AttrSb)
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
    Context 'FuncSelector' {
        Context 'attribute selector' {
            It 'pipes between attribute and attribute throws' {
                {
                    Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::Href), ([AngleParse.Attr]::Src)
                } | should -throw
            }
            It 'pipes between attribute and css selector throws' {
                {
                    Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::Href), 'div.some_class'
                } | should -throw
            }
            It 'pipes between attribute and property throws' {
                {
                    Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::Href), ([AngleParse.Prop]::Element)
                } | should -throw
            }
            It 'pipes between attribute and regex works' {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::ClassName), ([regex]'(\w+)')
                $result | should -be 'some_class', 'another_class'
            }
            It 'pipes between attribute and scriptblock works' {
                $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Attr]::ClassName), {
                    $_ -like 'some_class*' ? 1 : 2
                }
                $result | should -be 1
            }
        }
        Context 'css selector' {
            It 'pipes between css selector and attribute works' {
                $result = Get-Asset 'full.html' | Select-HtmlContent 'section#fragment > div.some_class', ([AngleParse.Attr]::ClassName)
                $result | should -be 'some_class', 'some_class'
            }
            It 'pipes between css selector and css selector works' {
                $result = Get-Asset 'full.html' | Select-HtmlContent 'section#fragment > div.some_class', 'a'
                $result.Length | should -be 10
            }
            It 'pipes between css selctor and property works' {
                $result = Get-Asset 'full.html' | Select-HtmlContent 'section#fragment > div.some_class', ([AngleParse.Prop]::Element)
                $result.Length | should -be 2
                $result | should -beOfType AngleSharp.Dom.IElement
            }
            It 'pipes between css selector and regex works' {
                $result = Get-Asset 'full.html' |
                        Select-HtmlContent 'section#fragment > div.some_class', ([regex]'Windows Server (\d{4})')

                $result | should -be 2003, 2008
            }
            It 'pipes between css selector and scriptblock works' {
                $result = Get-Asset 'full.html' | Select-HtmlContent 'section#fragment > div.some_class', {
                    $_ -match 'Windows Server' ? 1 : 2
                }
                $result | should -be 1, 2
            }
            Context 'property selector' {
                It 'pipes between property and attribute throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]::Element), ([AngleParse.Attr]::Href)
                    } | should -throw
                }
                It 'pipes between property and css selector throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]::Element), 'div.some_class'
                    } | should -throw
                }
                It 'pipes between property and property throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]::Element), ([AngleParse.Prop]::Element)
                    } | should -throw
                }
                It 'pipes between property and regex throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]::Element), ([regex]'(\w+)')
                    } | should -throw
                }
                It 'pipes between property and scriptblock works' {
                    $result = Get-Asset 'full-attribute.html' | Select-HtmlContent ([AngleParse.Prop]'TextContent'), {
                        $_ -like 'some link' ? 1 : 2
                    }
                    $result | should -be 1
                }
            }
            Context 'regex selector' {
                It 'pipes between regex and attribute throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([regex]'(\w+)'), ([AngleParse.Attr]::Href)
                    } | should -throw
                }
                It 'pipes between regex and css selector throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([regex]'(\w+)'), 'div.some_class'
                    } | should -throw
                }
                It 'pipes between regex and property throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent ([regex]'(\w+)'), ([AngleParse.Prop]::Element)
                    } | should -throw
                }
                It 'pipes between regex and regex works' {
                    $result = Get-Asset 'full.html' | Select-HtmlContent ([regex]'(\w+ \d{4})'), ([regex]'(\d{4})')
                    $result | should -be 2006, 2003, 2008, 2016, 2018
                }
                It 'pipes between regex and scriptblock works' {
                    $result = Get-Asset 'full.html' | Select-HtmlContent ([regex]'(\w+ \d{4})'), {
                        $_ -like 'November*' ? 1 : 2
                    }
                    $result | should -be 1, 2, 2, 2, 2
                }
            }
            Context 'scriptblock selector' {
                It 'pipes between scriptblock and attribute throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent { $_ -like 'some link' }, ([AngleParse.Attr]::Href)
                    } | should -throw
                }
                It 'pipes between scriptblock and css selector throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent { $_ -like 'some link' }, 'div.some_class'
                    } | should -throw
                }
                It 'pipes between scriptblock and property throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent { $_ -like 'some link' }, ([AngleParse.Prop]::Element)
                    } | should -throw
                }
                It 'pipes between scriptblock and regex throws' {
                    {
                        Get-Asset 'full-attribute.html' | Select-HtmlContent { $_ -like 'some link' }, ([regex]'(\w+)')
                    } | should -throw
                }
                It 'pipes between scriptblock and scriptblock works' {
                    $result = Get-Asset 'full-attribute.html' | Select-HtmlContent {
                        $_ -like 'some link' ? 1 : 2
                    }, { $_ * 2 }
                    $result | should -be 2
                }
            }
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
    Context 'RegexSelector' {
        It 'returns captured values when the regex matches' {
            $result = Get-Asset 'full.html' | Select-HtmlContent ([regex]'(\w+) (\d{4})')
            $result | should -be @(
                "November", "2006",
                "Server", "2003",
                "Server", "2008",
                "August", "2016",
                "January", "2018"
            )
        }
        It 'returns null when the regex has no capture groups' {
            $result = Get-Asset 'full.html' | Select-HtmlContent ([regex]'\w+ \d{4}')
            $result | should -be $null
        }
        It 'returns null when the regex does not match anything' {
            $result = Get-Asset 'full.html' | Select-HtmlContent ([regex]'(nonexistent)')
            $result | should -be $null
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