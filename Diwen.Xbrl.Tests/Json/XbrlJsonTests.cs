//
//  XbrlJsonTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Diwen.Xbrl.Json;
    using Xunit;

    public class XbrlJsonTests
    {
        private readonly ITestOutputHelper output;

        public XbrlJsonTests(ITestOutputHelper output)
        => this.output = output;

        [Theory]
        [InlineData("data/json/example.json")]
        public static void ImportJsonTest(string reportPath)
        {
            var report = Report.FromFile(reportPath);
            Assert.Equal("https://xbrl.org/2021/xbrl-json", report.DocumentInfo.DocumentType);
            Assert.Equal(new Uri("http://example.com/xbrl-json/taxonomy/example.xsd"), report.DocumentInfo.Taxonomy.First());
            Assert.Equal(new Uri("http://example.com/xbrl-json/taxonomy/"), report.DocumentInfo.Namespaces["eg"]);
            Assert.Equal(new Uri("http://standards.iso.org/iso/17442"), report.DocumentInfo.Namespaces["lei"]);
            Assert.Equal(new Uri("http://www.xbrl.org/2003/iso4217"), report.DocumentInfo.Namespaces["iso4217"]);
            Assert.Equal("f4", report.Facts["f3"].Links["footnote"]["_"].Single());
        }

        [Theory]
        [InlineData("data/example_output.json")]
        public static void ExportJsonTest(string reportPath)
        {
            var report = new Report
            {
                DocumentInfo = new()
                {
                    DocumentType = "https://xbrl.org/2021/xbrl-json",
                    Namespaces = new()
                    {
                        ["eg"] = new Uri("http://example.com/xbrl-json/taxonomy/"),
                        ["lei"] = new Uri("http://standards.iso.org/iso/17442"),
                        ["iso4217"] = new Uri("http://www.xbrl.org/2003/iso4217"),
                    },
                    Taxonomy =
                    [
                        new Uri("http://example.com/xbrl-json/taxonomy/example.xsd")
                    ]
                },
                Facts = new()
                {
                    ["f1"] = new()
                    {
                        Value = "1230000",
                        Decimals = 0,
                        Dimensions = new()
                        {
                            ["concept"] = "eg:Assets",
                            ["entity"] = "lei:00EHHQ2ZHDCFXJCPCL49",
                            ["period"] = "2020-01-01T00:00:00",
                            ["unit"] = "iso4217:EUR",
                        },
                    },
                    ["f2"] = new()
                    {
                        Value = "230000",
                        Decimals = 0,
                        Dimensions = new()
                        {
                            ["concept"] = "eg:Equity",
                            ["entity"] = "lei:00EHHQ2ZHDCFXJCPCL49",
                            ["period"] = "2020-01-01T00:00:00",
                            ["unit"] = "iso4217:EUR",
                        }
                    },
                    ["f3"] = new()
                    {
                        Dimensions = new()
                        {
                            ["concept"] = "eg:Equity",
                            ["entity"] = "lei:00EHHQ2ZHDCFXJCPCL49",
                            ["period"] = "2020-01-01T00:00:00",
                            ["unit"] = "iso4217:EUR",
                            ["language"] = "en-us",
                        },
                        Links = new()
                        {
                            ["footnote"] = new()
                            {
                                ["_"] = ["f4"],
                            }
                        },
                    },
                    ["f4"] = new()
                    {
                        Value = "This is a footnote",
                        Dimensions = new()
                        {
                            ["noteId"] = "f4",
                            ["concept"] = "xbrl:note",
                            ["language"] = "en",
                        },
                    },
                }
            };

            report.ToFile(reportPath);
        }


        [Theory]
        [InlineData("data/reference.xbrl")]
        [InlineData("data/ars.xbrl")]
        [InlineData("data/fp_ind_new_correct.xbrl")]
        public void XmlToJsonTest(string path)
        {
            //var xmlreport = Instance.FromFile(path, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            // my test data is awful
            //output.WriteLine($"{path}: {xmlreport.Facts.Count}");
            var xmlreport = Xbrl.Xml.Report.FromFile(path);

            var jsonreport = Report.FromXbrlXml(xmlreport);

            jsonreport.ToFile(Path.ChangeExtension(path, "json"));
        }

        [Theory]
        [InlineData("data/json/reference.json")]
        public void JsonToXmlTest(string path)
        {

            var dimensionDomainInfo = new Dictionary<string, string>
            {
                ["CS"] = "CS",
                ["CE"] = "ID",
            };

            var typedDomains = new HashSet<string>
            {
                "ID"
            };

            var typedDomainNamespace = KeyValuePair.Create("eba_typ", "http://www.eba.europa.eu/xbrl/crr/dict/typ");


            var jsonreport = Report.FromFile(path);

            var xmlreport = jsonreport.ToXbrlXml(dimensionDomainInfo, typedDomainNamespace, typedDomains);

            xmlreport.ToFile(Path.ChangeExtension(Path.GetFileName(path), ".xbrl"));
        }

        [Theory]
        [InlineData("data/json/5967007LIEEXZXHQPC18-2023-12-31-no.json")]
        public void DocumentInfoFeaturesTest(string path)
        {
            var report = Report.FromFile(path);
            Assert.NotNull(report.DocumentInfo.Features);
            report.ToFile(Path.GetFileName(path));
        }
    }
}
