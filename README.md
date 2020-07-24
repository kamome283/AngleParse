# AngleParse
Easy to use HTML parsing and processing tool for PowerShell.

```powershell
# Articles from PowerShell dev blog
iwr 'https://devblogs.microsoft.com/powershell/' |
Select-HtmlContent 'div.entry-box.container', @{
  Title = 'h5.entry-title'
  Author = 'span.entry-author-link', ([regex]'(\w+) \w+')
  PostDate = 'span.entry-post-date', { [DateTime]::Parse($_) }
} | select -first 3

# PostDate           Title                                                   Author
# --------           -----                                                   ------
# 2020/07/07 0:00:00 PowerShellGet 3.0 Preview 6 Release                     Sydney
# 2020/06/26 0:00:00 Native Commands in PowerShell – A New Approach – Part 2 Jim
# 2020/06/22 0:00:00 Native Commands in PowerShell – A New Approach          Jim
```

# Package
[https://www.powershellgallery.com/packages/AngleParse](https://www.powershellgallery.com/packages/AngleParse)

# Before Use
1. ```Install-Package AngleParse```
2. ```Import-Module AngleParse```

# Usage
```powershell
gc ./foobar.html -raw | Select-HtmlContent ([AngleParse.Attr]::Class)
```
`Select-HtmlContent` command receives string content from pipeline, interprets given content as HTML DOM tree, then processes with given selectors which are specified in the command's first argument.

# About Selector
There are 5 kinds of selectors.
They are CSS selector, attribute selector, regex selector, scriptblock selector and hashtable selector.
All selectors receive one input and output multiple (includes 0 and 1) items.
And by specifying multiple selectors, you can combine selectors which works as PowerShell's pipeline.
```powershell
'<div><span>abc</span></div>' | Select-HtmlContent "div > span", ([regex]'a(bc)')
# bc

# Similar work as below.
'<div><span>abc</span></div>' |
Select-HtmlContent "div > span" |
% { $_ | Select-HtmlContent ([regex]'a(bc)') }
# bc
```
When selector outputs single item, output array is unified like PowerShell's default behaviour for ease of use.
```powershell
> iwr "https://b.hatena.ne.jp/" | Select-HtmlContent "div.entrylist-contents", @{ 
>>     Title = "h3.entrylist-contents-title > a"
>>     Tags = "a[rel=tag]"
>> } | select -first 1 | Format-List

# Title contains only one string item so that array is unified.
# Title : Go To トラベル　感染を広げないためには(忽那賢志) - 個人 - Y...
# Tags  : {COVID-19, 旅行, 社会, 医療…}    
```

## CSS Selector
```powershell
'<div><span class="foo">text content here</span></div>' | Select-HtmlContent "div > span.foo"
# text content here
```
String is interpreted as CSS selector.
This selector receives DOM element and outputs DOM elements which matches given selector.

## Attribute Selector
```powershell
'<a href="https://foo.go.jp">bar</a>' | Select-HtmlContent ([AngleParse.Attr]::Href)
# https://foo.go.jp
```
Enum value of `AngleParse.Attr` class is interpreted as attribute selector.
There are 11 kinds of attributes as below.
```
Element
InnerHtml
OuterHtml
TextContent
Id
Class
SplitClasses
Href
Src
Title
Name
```
This selector receives DOM element and outputs matched attributes as string, 
excluding `Element` attribute which is introduced in ScriptBlock Selector section.
`SplitClasses` attribute outputs classes which is split by space, although `Class` attribute does not do any special work.
```powershell
# Class
'<div class="foo bar">' | Select-HtmlContent ([AngleParse.Attr]::Class)
# foo bar

# SplitClasses
'<div class="foo bar">' | Select-HtmlContent ([AngleParse.Attr]::SplitClasses)
# foo
# bar
```

## Regex Selector
Regex value is interpreted as regex selector.
This selector receives DOM element or string and outputs captured strings.
When you pass DOM element to this selector, this selector operate matching on the element's inner text content.
```powershell
# Not captured the day part so that outputs are year and month.
'<span>2020/07/22</span>' | Select-HtmlContent ([regex]'(\d{4})/(\d{2})/\d{2}')
# 2020
# 07
```

## ScriptBlock Selector
```powershell
'<span>7</span>' | Select-HtmlContent { [int]$_ * 6; [int]$_ * 7 }
# 42
# 49
```
ScriptBlock is interpreted as scriptblock selector.
This selector receives any kind of objects and outputs evaluated objects.
Passed object is bound to `$_`.
When you pass DOM element to this selector, the element is implicitly converted to the element's inner text content which has string type.
If you do not want this conversion, pipe `Element` attribute selector before using scriptblock selector.
```powershell
'<div><span>a</span><span>b</span></div>' | 
Select-HtmlContent ([AngleParse.Attr]::Element), { $_.ChildElementCount }
# 2
```

## Hashtable Selector
```powershell
'<div class="a">1a</div><div class="b">2b</div>' |
select-htmlcontent "> div",
@{ Class = ([AngleParse.Attr]::Class);
   NumPlus1 = ([regex]'(\d)\w'), { [int]$_ + 1 } }

# Class NumPlus1
# ----- --------
# a            2
# b            3
```
Hashtable is interpreted as hashtable selector.
Each value of the hashtable must be valid selctor(s).
This selector processes input with given selectors in the hashtable and bound to the corresponding key.

# Other Resources
[PowerShellから簡単にスクレイピングするためのツールを作った](https://qiita.com/kamome283/items/5b976a27ed203e959b09)(Japanese)

# Special Thanks To
* [AngleSharp](https://anglesharp.github.io/)

and all the support.
