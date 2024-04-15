namespace Diwen.Xbrl.Csv.Tests
{
    using System;
    using System.Linq;
    using Diwen.Xbrl.Json;
    using Xunit;
    using Xunit.Abstractions;

    public class XbrlJsonTests
    {
        private readonly ITestOutputHelper output;

        public XbrlJsonTests(ITestOutputHelper output)
        => this.output = output;

        [Theory]
        [InlineData("json/example.json")]
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
        [InlineData("example_output.json")]
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
    }
}