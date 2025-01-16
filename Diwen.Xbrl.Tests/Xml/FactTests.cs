//
//  FactTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2024 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

using System.Collections.Generic;
using System.Xml;

namespace Diwen.Xbrl.Tests.Xml
{
    using Diwen.Xbrl.Xml;
    using Xunit;

    public static class FactTests
    {
        [Fact]
        public static void FactWithNullPropertiesToString()
        {
            var fact = new Fact();
            Assert.NotEmpty(fact.ToString());
        }
        
        [Theory]
        [InlineData("x", "x", true)]
        [InlineData("x", null, false)]
        [InlineData(null, "x", false)]
        [InlineData(null, null, true)]
        public static void FactWithNullValue(string value1, string value2, bool expected) =>
            Assert.Equal(EqualityComparer<Fact>.Default.Equals(
                new Fact { Value = value1 }, 
                new Fact { Value = value2 }
                ), expected);
        
        [Theory]
        [InlineData("x", "x", true)]
        [InlineData("x", null, false)]
        [InlineData(null, "x", false)]
        [InlineData(null, null, true)]
        public static void FactWithNullMetric(string value1, string value2, bool expected) =>
            Assert.Equal(EqualityComparer<Fact>.Default.Equals(
                new Fact { Metric = new XmlQualifiedName(value1) }, 
                new Fact { Metric = new XmlQualifiedName(value2) }
            ), expected);
        
        [Theory]
        [InlineData("x", "x", true)]
        [InlineData("x", null, false)]
        [InlineData(null, "x", false)]
        [InlineData(null, null, true)]
        public static void FactWithNullDecimals(string value1, string value2, bool expected) =>
            Assert.Equal(EqualityComparer<Fact>.Default.Equals(
                new Fact { Decimals = value1 }, 
                new Fact { Decimals = value2 }
            ), expected);


    }
}
