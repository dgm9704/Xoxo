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

        [Fact]
        public static void FactWithNullValueRemove()
        {
            var fact = new Fact();
            var facts = new FactCollection(report: null) { fact };

            for (var i = facts.Count - 1; i >= 0; i--)
            {
                var f = facts[i];
                if (string.IsNullOrEmpty(f.Value))
                    facts.Remove(f);
            }
        }
    }
}
