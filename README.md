[![.NET](https://github.com/dgm9704/Xoxo/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dgm9704/Xoxo/actions/workflows/dotnet.yml)
[![nuget](https://img.shields.io/nuget/v/Diwen.Xbrl.svg)](https://www.nuget.org/packages/Diwen.Xbrl/)

# Xoxo
Container for the Diwen.Xbrl library and any related stuff

## Diwen.Xbrl
A .NET library for reading, writing, comparing and converting XBRL reports
Supports:

- EBA and EIOPA ITS reporting (eg COREP, FINREP, AE, FP, SBP, Solvency II, Pension Funds, etc.)

- Australian and Finnish SBR message structures

| format               | read/write    | convert to          |
| -------------------- | ------------- | -------             |
| xBRL-XML             | read+write    | xBRL-CSV, xBRL-JSON |
| xBRL-CSV             | read+write    | xBRL-XML            |
| xBRL-CSV "Plain-csv" | read+write    | xBRL-XML            |
| xBRL-JSON            | read+write    | xBRL-XML            |
| iXBRL                | read          | xBRL-XML            |


### License:

GNU Lesser General Public License v3.0

http://www.gnu.org/licenses/gpl.txt

http://www.gnu.org/licenses/lgpl.txt


TLDR; 

You can use the compiled library with your application without it affecting the licensing of your other source code. 

(Just remember to link back here ie. "attribution")

If you modify the source code and distribute it, you need to license the software containing the modified version accordingly.

## Diwen.Xbrl.Tests
Tests for exercising Diwen.Xbrl code, also serving as documentation and samples

### Test code license 
[Free Public License 1.0.0](https://opensource.org/licenses/FPL-1.0.0)


# My environment
Code is written in [C#](https://docs.microsoft.com/en-us/dotnet/csharp/index), 

targeting [.NET 8](https://learn.microsoft.com/en-us/dotnet/api/?view=net-8.0)

Test framework is [xUnit](https://xunit.net/)

I use [Visual Studio Code](https://code.visualstudio.com/) 

on [Arch Linux](https://www.archlinux.org/)

## Ways to get in touch
If you have any questions, comments, suggestions, problems, bugreports/fixes, etc

### create an issue 
It's always ok to create one even if you're not sure

https://github.com/dgm9704/Xoxo/issues/new

### send a message through the NuGet package page
This might be the best way if you have something you don't want to share publicly

[https://www.nuget.org/packages/Diwen.Xbrl/3.4.2/ContactOwners](https://www.nuget.org/packages/Diwen.Xbrl/3.5.0/ContactOwners)

### mastodon: @diwen@techhub.social
https://techhub.social/@diwen

### bluesky: @diwen.social
https://bsky.app/profile/diwen.social

# Contributing
Thank you for your interest in the project!

Due to to my old-school opinionated workflow, I don't currently accept unsolicited pull requests.

Please instead create an issue with the following:
- Details about what is wrong or missing, exception details etc.
- Link to applicable documentation (taxonomy, Filing Rules, etc )
- A (preferably minimal) report or link to report that can be used to reproduce and test the issue

I will take a look and we can have a public or private discussion if needed.

If I think that the fix or change is something that should go in the library, 

I will then implement it, taking into consideration any suggestions.

