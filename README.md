[![.NET Core](https://github.com/dgm9704/Xoxo/workflows/.NET%20Core/badge.svg)](https://github.com/dgm9704/Xoxo/actions/workflows/dotnetcore.yml)
[![nuget](https://img.shields.io/nuget/v/Diwen.Xbrl.svg)](https://www.nuget.org/packages/Diwen.Xbrl/)

# Default branch has been renamed
Use these commands to update:
```
git branch -m master main
git fetch origin
git branch -u origin/main main
git remote set-head origin -a
```

# Xoxo
Container for the Diwen.Xbrl library and any related stuff

## Please get in touch
If you have any questions, comments, suggestions, problems, etc

### create an issue
https://github.com/dgm9704/Xoxo/issues/new

### twitter: @DiwenXbrl
https://twitter.com/DiwenXbrl

### send a message through the NuGet package page
https://www.nuget.org/packages/Diwen.Xbrl/1.0.4/ContactOwners

## Diwen.Xbrl
A .NET library for reading, writing and comparing XBRL documents (instances)
Supports:

- EBA and EIOPA ITS reporting (eg COREP, FINREP, AE, FP, SBP, Solvency II, Pension Funds, etc.)

- Australian and Finnish SBR message structures

- (tentative) parsing of Inline XBRL from ESMA ESEF

### License:

GNU Lesser General Public License v3.0

http://www.gnu.org/licenses/gpl.txt

http://www.gnu.org/licenses/lgpl.txt

## Diwen.Xbrl.Tests
Tests for exercising Diwen.Xbrl code, also serving as documentation and samples

### License

[Free Public License 1.0.0](https://opensource.org/licenses/FPL-1.0.0)


# Environment
Code is written in [C#](https://docs.microsoft.com/en-us/dotnet/csharp/index), targeting 
[.NET Standard 2.0](https://github.com/dotnet/standard/blob/v2.1.0/docs/versions/netstandard2.0.md)

Test framework is [xUnit](https://xunit.net/)
and test code is targeting [.NET 6](https://docs.microsoft.com/en-us/dotnet/)

I use [Visual Studio Code](https://code.visualstudio.com/) 
on [Arch Linux](https://www.archlinux.org/)



