[![.NET](https://github.com/dgm9704/Xoxo/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dgm9704/Xoxo/actions/workflows/dotnet.yml)
[![nuget](https://img.shields.io/nuget/v/Diwen.Xbrl.svg)](https://www.nuget.org/packages/Diwen.Xbrl/)

# Branched!
The library code is now in two branches:
- main: .NET 6+ version that will get new features etc. (version 2 upwards)
- legacy: .NET Standard 2.0 version that will only get critical bug fixes (versions 1.x)

# Xoxo
Container for the Diwen.Xbrl library and any related stuff


## Diwen.Xbrl
A .NET library for reading, writing and comparing XBRL documents (instances)
Supports:

- EBA and EIOPA ITS reporting (eg COREP, FINREP, AE, FP, SBP, Solvency II, Pension Funds, etc.)

- Australian and Finnish SBR message structures

- Parsing of Inline XBRL to "normal" instance (from ESMA ESEF)

- Bare-bones implementation of reading and writing xBRL-CSV and converting to/from xBRL-XML


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

### License

[Free Public License 1.0.0](https://opensource.org/licenses/FPL-1.0.0)


# Environment
Code is written in [C#](https://docs.microsoft.com/en-us/dotnet/csharp/index), targeting 
[.NET 6](https://learn.microsoft.com/en-us/dotnet/api/?view=net-6.0)

Test framework is [xUnit](https://xunit.net/)
and test code is targeting [.NET 6](https://docs.microsoft.com/en-us/dotnet/)

I use [Visual Studio Code](https://code.visualstudio.com/) 
on [Arch Linux](https://www.archlinux.org/)

## Ways get in touch
If you have any questions, comments, suggestions, problems, bugreports/fixes, etc

### create an issue 
It's always ok to create one even if you're not sure

https://github.com/dgm9704/Xoxo/issues/new

### send a message through the NuGet package page
This might be the best way if you have a complicated issue or something you don't want share publicly

https://www.nuget.org/packages/Diwen.Xbrl/1.2.0/ContactOwners

### twitter: @DiwenXbrl
https://twitter.com/DiwenXbrl


# (old news) Default branch has been renamed
Use these commands to update:
```
git branch -m master main
git fetch origin
git branch -u origin/main main
git remote set-head origin -a
```
