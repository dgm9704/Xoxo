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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Xunit;

    public static class InlineXbrlTests
    {

        [Fact]
        public static void ReadInlineXbrl()
        {
            var file = "esma/G2-1-2.xhtml";
            var doc = XDocument.Load(file);
        }

        [Fact]
        public static void ParseInlineXbrlContexts()
        {
            var file = "esma/G2-1-2.xhtml";
            var doc = XDocument.Load(file);
            var xbrli = doc.Root.GetNamespaceOfPrefix("xbrli");
            var contexts = doc.Root.Descendants(xbrli + "context");
            var xml = new XmlSerializer(typeof(Context));
            foreach (var c in contexts)
            {
                var stream = new MemoryStream();
                c.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var context = (Context)xml.Deserialize(stream);
            }
        }

        [Fact]
        public static void ParseInlineXbrlUnits()
        {
            var file = "esma/G2-1-2.xhtml";
            var doc = XDocument.Load(file);
            var xbrli = doc.Root.GetNamespaceOfPrefix("xbrli");
            var units = doc.Root.Descendants(xbrli + "unit");
            var xml = new XmlSerializer(typeof(Unit));
            foreach (var u in units)
            {
                var stream = new MemoryStream();
                u.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var unit = (Unit)xml.Deserialize(stream);
            }
        }

        [Fact]
        public static void ParseInlineXbrlFacts()
        {
            var file = "esma/G2-1-2.xhtml";
            var doc = XDocument.Load(file);

            var facts = doc.Descendants().Where(d => d.Attribute("contextRef") != null);
            foreach (var f in facts)
            {
                var name = f.Attribute("name").Value;
                var prefix = name.Split(':').First();
                var ns = doc.Root.GetNamespaceOfPrefix(prefix).ToString();
                var unitRef = f.Attribute("unitRef")?.Value;
                var decimals = f.Attribute("decimals")?.Value;
                var value = f.Value;
                var scale = f.Attribute("scale")?.Value;

                if (!string.IsNullOrWhiteSpace(scale))
                {
                    var power = int.Parse(scale);
                    var v = decimal.Parse(value.Replace(" ",""));
                    var multiplier = (decimal)Math.Pow(10, power);
                    v *= multiplier;
                    value = v.ToString();
                }

                var contextRef = f.Attribute("contextRef").Value;
                var fact = new Fact(name, ns, unitRef, decimals, contextRef, value);
            }

        }

    }
}