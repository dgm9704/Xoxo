//
//  ScenarioTests.cs
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

namespace Diwen.Xbrl.Tests.Csv
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Diwen.Xbrl.Csv;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Diwen.Xbrl.Extensions;
    using Xunit;
    using Xunit.Abstractions;

    public class XbrlCsvPlainTests
    {
        private readonly ITestOutputHelper output;

        public XbrlCsvPlainTests(ITestOutputHelper output)
        => this.output = output;

        public static Dictionary<string, string> ReadFilingIndicatorInfo(string file)
        => File.ReadAllLines(Path.Combine("data/csv", file)).
            Select(l => l.Split(',')).
            ToDictionary(x => x[0], x => x[1]);

        [Theory]
        [InlineData("data/csv/DUMMYLEI123456789012.CON_FR_DORA010100_DORA_2024-12-31_20241213174803429.zip")]
        public void PlainCsvToXmlTest(string reportPath)
        => PlainCsvToXml(reportPath);

        public static string PlainCsvToXml(string reportPath)
        {

            var entrypoint = PlainCsvReport.GetPackageEntryPoint(reportPath).Replace(@"http://", "");

            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var tableDefinitions = moduleDefinition.TableDefinitions();

            var plainCsvReport = PlainCsvReport.FromFile(reportPath, tableDefinitions);

            var dimensionDomainInfo = ReadDimensionDomainInfo("EBA40_DimensionDomain.csv");

            var typedDomains = ReadTypedDomainInfo("EBA40_TypedDomain.csv");

            var typedDomainNamespace = KeyValuePair.Create("eba_typ", "http://www.eba.europa.eu/xbrl/crr/dict/typ"); //???

            var filingIndicators = ReadFilingIndicatorInfo("EBA40_dora_FilingIndicators.csv");

            var xmlReport = plainCsvReport.ToXbrlXml(tableDefinitions, dimensionDomainInfo, typedDomainNamespace, filingIndicators, typedDomains, moduleDefinition);

            var xmlReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".xbrl");
            xmlReport.ToFile(xmlReportPath);
            return xmlReportPath;

        }


        [Theory]
        [InlineData("data/csv/DUMMYLEI123456789012.CON_FR_DORA010100_DORA_2024-12-31_20241210113351223.xbrl")]
        public void XmlToPlainCsvTest(string reportPath)
        => XmlToPlainCsv(reportPath);

        public static string XmlToPlainCsv(string reportPath)
        {
            var xmlReport = Xbrl.Xml.Report.FromFile(reportPath);

            var entrypoint = Path.ChangeExtension(xmlReport.SchemaReference.Value.Replace("http://", ""), "json");

            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var tableDefinitions = moduleDefinition.TableDefinitions();

            var filingIndicators = ReadFilingIndicatorInfo("EBA40_dora_FilingIndicators.csv");

            var plainCsvReport = xmlReport.ToXbrlCsvPlain(tableDefinitions, filingIndicators, moduleDefinition);

            var csvReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".zip");

            plainCsvReport.Export(csvReportPath, tableDefinitions);

            return csvReportPath;
        }

        public static HashSet<string> ReadTypedDomainInfo(string path)
       => File.ReadAllLines(Path.Combine("data/csv", path)).ToHashSet();

        public static Dictionary<string, string> ReadDimensionDomainInfo(string file)
        => File.ReadAllLines(Path.Combine("data/csv", file)).
            Select(l => l.Split(',')).
            ToDictionary(x => x[0], x => x[1]);
    }
}