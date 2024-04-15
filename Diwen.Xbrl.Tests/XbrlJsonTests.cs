namespace Diwen.Xbrl.Csv.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Diwen.Xbrl.Json;
    using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
    using Xunit;
    using Xunit.Abstractions;

    public class XbrlJsonTests
    {
        private readonly ITestOutputHelper output;

        public XbrlJsonTests(ITestOutputHelper output)
        => this.output = output;

        [Theory]
        [InlineData("json/example.json")]
        public static void ImportTest(string reportPath)
        {
            var report = Report.FromFile(reportPath);
            Assert.Equal("https://xbrl.org/2021/xbrl-json", report.DocumentInfo.DocumentType);
            Assert.Equal(new Uri("http://example.com/xbrl-json/taxonomy/example.xsd"), report.DocumentInfo.Taxonomy.First());
            Assert.Equal(new Uri("http://example.com/xbrl-json/taxonomy/"), report.DocumentInfo.Namespaces["eg"]);
            Assert.Equal(new Uri("http://standards.iso.org/iso/17442"), report.DocumentInfo.Namespaces["lei"]);
            Assert.Equal(new Uri("http://www.xbrl.org/2003/iso4217"), report.DocumentInfo.Namespaces["iso4217"]);
        }

        [Theory]
        [InlineData("example_output.json")]
        public static void ExportJsonTest(string reportPath)
        {
            var report = new Report
            {
                DocumentInfo =
                {
                    DocumentType = "https://xbrl.org/2021/xbrl-json",
                    Namespaces =
                    {
                        ["eg"]= new Uri("http://example.com/xbrl-json/taxonomy/"),
                        ["lei"] = new Uri("http://standards.iso.org/iso/17442"),
                        ["iso4217"] = new Uri("http://www.xbrl.org/2003/iso4217"),
                    },
                    Taxonomy =
                    [
                        new Uri("http://example.com/xbrl-json/taxonomy/example.xsd"),
                    ]
                },
                Facts =
                {
                     ["f1"] = new Fact
                     {
                         Value = "1230000",
                         Decimals = 0,
                         Dimensions =
                         {
                             ["concept"] = "eg:Assets",
                             ["entity"] = "lei:00EHHQ2ZHDCFXJCPCL49",
                             ["period"] = "2020-01-01T00:00:00",
                             ["unit"] = "iso4217:EUR",
                         },
                     },
                     ["f2"] = new Fact
                     {
                         Value = "230000",
                         Decimals = 0,
                         Dimensions =
                         {
                             ["concept"] = "eg:Equity",
                             ["entity"] = "lei:00EHHQ2ZHDCFXJCPCL49",
                             ["period"] = "2020-01-01T00:00:00",
                             ["unit"] = "iso4217:EUR",
                         }
                     },
                },
            };

            report.ToFile(reportPath);
        }
    }
}