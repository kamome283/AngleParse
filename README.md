# AngleParse

An easy‑to‑use HTML parsing and processing toolkit for PowerShell.

```powershell
# Popular modules in PowerShell Gallery

Invoke-WebRequest 'https://www.powershellgallery.com' |
Select-HtmlContent 'div.stats-table tr:not(:first-child)' @{
  Module = 'td.text-left', { $_.Trim() }
  Downloaded = 'td.text-right', { [long]($_ -replace ',', '') }
  Page = 'td.text-left a', ([AngleParse.Attr]'href'), { "https://www.powershellgallery.com$_" }
} { [pscustomobject]$_ }

# Output:
# Page                                                          Downloaded Module
# ----                                                          ---------- ------
# https://www.powershellgallery.com/packages/Az.Accounts/         17969207 Az.Accounts
# https://www.powershellgallery.com/packages/DellBIOSProvider/    12883950 DellBIOSProvider
# https://www.powershellgallery.com/packages/PackageManagement/    5033505 PackageManagement
# https://www.powershellgallery.com/packages/Az.Storage/           4769681 Az.Storage
# https://www.powershellgallery.com/packages/Az.Resources/         4608517 Az.Resources
```

## Table of Contents

- [Overview](#overview)
- [Breaking Changes before v0.4](#breaking-changes-before-v04)
- [Installation](#installation)
- [Usage](#usage)
- [How selectors work](#how-selectors-work)
- [Selector Reference](#selector-reference)
    - [CSS Selector](#css-selector)
    - [Attribute Selector](#attribute-selector)
    - [Property Selector](#property-selector)
    - [Regex Selector](#regex-selector)
    - [ScriptBlock Selector](#scriptblock-selector)
    - [Table Selector](#table-selector)
- [FAQ](#faq)
- [License](#license)

## Overview

AngleParse simplifies HTML parsing and data extraction in PowerShell.
Built on robust C# and the AngleSharp library, it provides intuitive, PowerShell‑style
processing toolkit.
Seamlessly integrates with common cmdlets like:
- `Invoke-WebRequest` – retrieve web pages
- `Get-Content` – read local HTML files
- `Select-Object` – format and filter output
- `ConvertTo-*` – transform output into other formats

## Breaking Changes before v0.4

- The `-Selector` parameter now has the `ValueFromRemainingArguments` attribute.
- The `-Selector` parameter is no longer a positional parameter.
- The `-Content` parameter is no longer a positional parameter.
- `[AngleParse.Attr]::Element` has been deprecated. Use `[AngleParse.Prop]::Element` instead.

```powershell
## This is how it used to be before v0.4
'Some HTML Document' | Select-HtmlContent 'div', ([AngleParse.Attr]::Element)

## This is how it is now
'Some HTML Document' | Select-HtmlContent 'div' ([AngleParse.Prop]::Element)
```


## Installation

```powershell
Install-Package AngleParse
Import-Module AngleParse
```

## Usage

The `Select-HtmlContent` command reads a string from the pipeline or `-Content` parameter, parses it as an HTML DOM, and applies the specified selectors which are in forms of variadic parameters.

```powershell
Get-Content example.html -Raw |
  Select-HtmlContent 'div.foo a.bar' ([AngleParse.Attr]::Href)
```

## How selectors work

There are 6 types of selectors:
CSS selector, Attribute selector, Property selector, Regex selector, ScriptBlock selector, and Table selector.
Each selector inputs one item and outputs zero to many items.
By specifying multiple selectors, you can combine them to work like PowerShell's pipeline.

```selector_work.html
<!DOCTYPE html>
<html lang="en">
<head><meta charset="UTF-8"><title>Shop</title></head>
<body>
<h1>Shop</h1>
<div id="products">
    <div class="product">
        <h2>Product 1</h2>
        <p>Price: $10</p>
        <button class="add-to-cart">Add to Cart</button>
    </div>
    <div class="product">
        <h2>Product 2</h2>
        <p>Price: $20</p>
        <button class="add-to-cart">Add to Cart</button>
    </div>
</div>
</body>
</html>
```

```powershell
filter makeOutputInTableFormat { [pscustomobject]$_ }

Get-Content selector_work.html -raw |
  Select-HtmlContent 'div#products > div.product' @{
      Name = 'h2'
      Price = 'p', ([regex]'\$(\d+)')
  } | makeOutputInTableFormat
# Output:
# Name      Price
# ----      -----
# Product 1 10
# Product 2 20
```

There are 3 kinds of input and output types:
- **Element**: a DOM element and subtype of string
- **String**: a string and subtype of object
- **Object**: any object

Each selector has its own input and output types.
And if its input type is not matched with previous output type,
it will throw an error when this cmdlet tries to connect them, not on the actual
processing stage.

```powershell
# This code throws an error because the first selector outputs string,
# but the second selector requires Element as its input type which is subtype of string.
Get-Content selector_work.html -raw |
  Select-HtmlContent ([regex]'Price: \$(\d+)') 'p'
```

## Selector Reference

### CSS Selector

*Element -> Element*

a CSS selector receives DOM elements and output DOM elements that match the given CSS selector expression.
Any bare string is interpreted as a CSS selector.

```css_selector.html
<div>
  <span class="foo">text content here</span>
</div>
```

```powershell
Get-Content css_selector.html -raw |
  Select-HtmlContent "div > span.foo"
# Output: 'text content here'
```

### Attribute Selector

*Element -> String*

An attribute selector receives DOM elements and outputs matched attributes as strings.
There are some already defined attributes like:
- `Href`
- `Src`
- `Title`
- `Name`

If you want to access to other attributes, you can make your own attribute selector by
converting from string using `[AngleParse.Attr]` class.
(e.g. `([AngleParse.Attr]'some-attribute')`)

```attribute_selector.html
<a href="https://example.com" some-attribute="hey"><span>some link</span></a>
```

```powershell
Get-Content attribute_selector.html -raw |
  Select-HtmlContent ([AngleParse.Attr]::Href)
# Output: https://example.com

Get-Content attribute_selector.html -raw |
  Select-HtmlContent ([AngleParse.Attr]'some-attribute')
# Output: hey
```

If you access a valueless attribute, it will return an empty string.
And if you access an attribute that doesn't exist, it will return `$null`.

There are also some special selectors in this category, which are actually not attributes but are useful for HTML processing.
They are:
- `InnerHtml` - the inner HTML of the element
- `OuterHtml` - the outer HTML of the element
- `TextContent` - the text content of the element
- `Id` - the ID of the element
- `ClassName` - the class name of the element
- `SplitClasses` - array of class names split by space

```powershell
Get-Content attribute_selector.html -raw |
  Select-HtmlContent ([AngleParse.Attr]::InnerHtml)

# Output: <span>some link</span>
```

### Property Selector

*Element -> Object*

A property selector receives DOM elements and outputs the property value of the inner
`AngleSharp.Dom.IElement` by acessing dynamically.
This selector is useful when you want to access the `IElement` property of the DOM element.
You can make a property selector by converting from string using `[AngleParse.Prop]` class.
(e.g. `([AngleParse.Prop]'some-property')`)

```property_selector.html
<div><span class="foo">text content here</span></div>
```

```powershell
# Well, I know that you should use [AngleParse.Attr]::TextContent instead of this. 
# This is just an example.
Get-Content property_selector.html -raw |
  Select-HtmlContent ([AngleParse.Prop]'TextContent')
# Output: text content here
```

As like the attribute selector, there are some special properties in this category.
They are:
- `Element` - the inner `AngleSharp.Dom.IElement` of the DOM element
- `AttributesTable` - the attributes of the element as a dictionary


### Regex Selector

*String -> String*

This selector receives string and outputs captured strings.
When you pass DOM element to this selector, it operates capturing on the element's inner text content.
Regex value is interpreted as regex selector.

```regex_selector.html
<div><span>2020/07/22</span></div>
```

```powershell
Get-Content regex_selector.html -raw |
  Select-HtmlContent ([regex]'(\d{4})/(\d{2})')
# Output: 2020, 07
```

### ScriptBlock Selector

*Object -> Object*

This selector receives any object and outputs the result of the script block.
This selector is useful when you want to process on the scraped data.
In the script block, you can use `$_` to refer to the current object.
When you pass DOM element to this selector, it operates on the inner text content of the element.

```scriptblock_selector.html
<span class="some-date">2025/05/04</span>
```

```powershell
Get-Content scriptblock_selector.html -raw |
  Select-HtmlContent { [DateTime]$_ }
# Output: 2025/05/04 0:00:00
```

### Table Selector

*T -> Object where T is the most strict type required in the each branch*

Table selector outputs hashtable composed of the given key-value pairs, whose values are
processed by the given selectors in each branch.
The input of this selector required to conform the most strict type among the selectors in the hashtable requires to conform.
Hashtables are interpreted as table selectors.

```table_selector.html
<body>
  <div class="a">
    1a
  </div>
  <div class="b">
    2b
  </div>
</body>
```

```powershell
Get-Content table_selector.html -raw |
  Select-HtmlContent @{
    ClassName = ([AngleParse.Attr]::ClassName);
    NumPlus1 = ([regex]'(\d)\w'), { [int]$_ + 1 }
  }
# Output:
# Class Number
# ----- ------
# a     2
# b     3

# This throws an error because the input type is string,
# though the most strict type required in the each branch is Element that is subtype of string.
# This does not conform to type constraint.
Get-Content table_selector.html -raw |
  Select-HtmlContent ([regex]'.*') @{
    ClassName = ([AngleParse.Attr]::ClassName);
    NumPlus1 = ([regex]'(\d)\w'), { [int]$_ + 1 }
  }
```

## FAQ

**Q: Why does the output is not in array when the output is a single item?**

This is because of the unification. This emulates PowerShell's default behavior for ease of use.


```unification.html
<body>
  <div class="entrylist-contents">
    <h3 class="entrylist-contents-title">
      <a href="https://example.com">Example</a>
    </h3>
    <a rel="tag">Tag1</a>
    <a rel="tag">Tag2</a>
  </div>
</body>
```

```powershell
Get-Content unification.html -raw |
  Select-HtmlContent "div.entrylist-contents" @{
    Title = "h3.entrylist-contents-title > a"
    Tags  = "a[rel=tag]" 
  } { [pscustomobject]$_ }
# Output:
# Title : Example
# Tags  : {Tag1, Tag2}

# Did you see that the Title contains only one string item, not a string array?
# This is because the output array is unified.
```


## License

Apache License 2.0
