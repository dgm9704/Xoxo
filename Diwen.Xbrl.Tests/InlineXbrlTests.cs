//
//  InlineXbrlTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;

    public static class InlineXbrlTests
    {

        [Fact]
        public static void ParseInlineXbrlDocument()
        {
            var inputFile = "esma/G2-1-2.xhtml";
            var instance = InlineXbrl.ParseInstance(inputFile);
            var outputFile = Path.ChangeExtension(inputFile, "xbrl");
            instance.ToFile(outputFile);
        }

        [Fact]
        public static void ValidateInlineXbrlDocument()
        {
            var inputFile = "esma/G2-1-2.xhtml";
            var result = InlineXbrl.Validate(inputFile, InlineXbrlType.Esef);
            Assert.False(result.Messages.Any(), string.Join("\n", result.Messages));
        }

        [Fact]
        public static void ValidateUnknownInlineXbrlDocument()
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => InlineXbrl.Validate("inputFile", (InlineXbrlType)42));
    }

}