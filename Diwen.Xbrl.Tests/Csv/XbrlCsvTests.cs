//
//  XbrlCsvTests.cs
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

namespace Diwen.Xbrl.Tests.Csv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Diwen.Xbrl.Xml.Comparison;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Diwen.Xbrl.Extensions;
    using Xunit;
    using Diwen.Xbrl.Csv;
    using System.Xml.Linq;

    public class XbrlCsvTests
    {
        private readonly ITestOutputHelper output;

        public XbrlCsvTests(ITestOutputHelper output)
        => this.output = output;

        [Fact]
        public void ExportTests()
        {
            var entrypointUrl = "http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2021-07-15/mod/sbp_cr_con.json";

            var moduleDefinition = ModuleDefinition.FromFile(entrypointUrl.Replace("http://", "taxonomy/"));

            var report = new Report
            {
                Entrypoint = entrypointUrl,
                Parameters = new Dictionary<string, string>
                {
                    ["entityID"] = "lei:DUMMYLEI123456789012",
                    ["refPeriod"] = "2021-12-31",
                    ["baseCurrency"] = "iso4217:EUR",
                    ["decimalsInteger"] = "0",
                    ["decimalsMonetary"] = "-3",
                    ["decimalsPercentage"] = "4",
                    ["decimalsDecimal"] = "2",
                },
                FilingIndicators = new Dictionary<string, bool>
                {
                    ["C_101.00"] = false,
                    ["C_102.00"] = false,
                    ["C_103.00"] = false,
                    ["C_105.01"] = false,
                    ["C_105.02"] = true,
                    ["C_105.03"] = true,
                    ["C_111.00"] = false,
                    ["C_112.00"] = false,
                    ["C_113.00"] = true,
                    ["S_00.01"] = true,
                },
            };

            //datapoint,factValue
            report.AddData("S_00-01", "dp31870", "eba_AS:x1");
            report.AddData("S_00-01", "dp37969", "eba_SC:x6");

            // datapoint,factValue,IRN
            report.AddData("C_105-03", "dp434188", "grarenmw", "IRN", "36");
            report.AddData("C_105-03", "dp434189", "eba_GA:AL", "IRN", "36");
            report.AddData("C_105-03", "dp434188", "grarenmw2", "IRN", "8");
            report.AddData("C_105-03", "dp434189", "eba_GA:AL", "IRN", "8");

            // datapoint,factValue,IMI,PBE
            report.AddData("C_105-02", "dp439585", "250238.28", ("IMI", "ksnpfnwn"), ("PBI", "ksnpfnwn"));
            report.AddData("C_105-02", "dp439586", "247370.72", ("IMI", "ksnpfnwn"), ("PBI", "ksnpfnwn"));
            report.AddData("C_105-02", "dp439585", "250238.28", ("IMI", "kotnyngp"), ("PBI", "kotnyngp"));
            report.AddData("C_105-02", "dp439586", "247370.72", ("IMI", "kotnyngp"), ("PBI", "kotnyngp"));


            // datapoint,factValue,FTY,INC
            report.AddData("C_113-00", "dp439732", "304132.94", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439750", "eba_IM:x33", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439744", "0.1", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439745", "0.72", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439751", "0.34", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439752", "0.46", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439753", "eba_ZZ:x409", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113-00", "dp439732", "304132.94", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113-00", "dp439750", "eba_IM:x33", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113-00", "dp439744", "0.1", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113-00", "dp439745", "0.72", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113-00", "dp439751", "0.34", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113-00", "dp439752", "0.46", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113-00", "dp439753", "eba_ZZ:x409", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));

            report.Export("DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000", moduleDefinition);
        }

        [Theory]
        [InlineData("DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000.zip")]
        public static void ReadPackageTest(string packageName)
        {
            var packagePath = Path.Combine("data", "csv", packageName);
            var reportFiles = Report.ReadPackage(packagePath);

            var metafolder = "META-INF";
            var reportfolder = "reports";

            Assert.True(reportFiles.ContainsKey($"{metafolder}/reports.json"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/report.json"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/parameters.csv"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/FilingIndicators.csv"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/S_00.01.csv"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/C_105.02.csv"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/C_105.03.csv"));
            Assert.True(reportFiles.ContainsKey($"{reportfolder}/C_113.00.csv"));
        }

        [Theory]
        [InlineData("DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000.zip")]
        public static void ImportTest(string packageName)
        {

            var packagePath = Path.Combine("data", "csv", packageName);
            var entrypoint = PlainCsvReport.GetPackageEntryPoint(packagePath).Replace(@"http://", "taxonomy/");

            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var report = Report.FromFile(packagePath, moduleDefinition);

            Assert.Equal("http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2021-07-15/mod/sbp_cr_con.json", report.Entrypoint);
            Assert.Equal("lei:DUMMYLEI123456789012", report.Parameters["entityID"]);
            Assert.Equal("2021-12-31", report.Parameters["refPeriod"]);
            Assert.Equal("iso4217:EUR", report.Parameters["baseCurrency"]);
            Assert.Equal("0", report.Parameters["decimalsInteger"]);
            Assert.Equal("-3", report.Parameters["decimalsMonetary"]);
            Assert.Equal("4", report.Parameters["decimalsPercentage"]);
            Assert.Equal("2", report.Parameters["decimalsDecimal"]);

            Assert.False(report.FilingIndicators["C_101.00"]);
            Assert.False(report.FilingIndicators["C_102.00"]);
            Assert.False(report.FilingIndicators["C_103.00"]);
            Assert.False(report.FilingIndicators["C_105.01"]);
            Assert.True(report.FilingIndicators["C_105.02"]);
            Assert.True(report.FilingIndicators["C_105.03"]);
            Assert.False(report.FilingIndicators["C_111.00"]);
            Assert.False(report.FilingIndicators["C_112.00"]);
            Assert.True(report.FilingIndicators["C_113.00"]);
            Assert.True(report.FilingIndicators["S_00.01"]);
        }

        [Theory]
        [InlineData("data/csv/DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.xbrl")]
        [InlineData("data/csv/F_18-00-a.xbrl")]
        public void XmlToCsvTest(string reportPath)
        => XmlToCsv(reportPath);

        [Theory]
        [InlineData("data/csv/DUMMYLEI123456789012.CON_FR_GSII010300_GSII_2026-12-31_20260108135514790.zip")]
        [InlineData("data/csv/DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.zip")]
        [InlineData("data/csv/F_18-00-a.zip")]
        public void CsvToXmlTest(string reportPath)
        => CsvToXml(reportPath);

        [Theory]
        [InlineData("data/csv/DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.xbrl")]
        [InlineData("data/csv/FINREP_F_23-01_R0080_C0010.xbrl")]
        [InlineData("data/csv/FINREP_F_40-01_R999_C0031.xbrl")]
        [InlineData("data/csv/c299_mi53.xbrl")]
        public void XmlToCsvToXml(string xmlInPath)
        {
            var csvPath = XmlToCsv(xmlInPath);
            var xmlOutPath = CsvToXml(csvPath);
            var result = ReportComparer.Report(xmlInPath, xmlOutPath);
            if (!result.Result)
                File.WriteAllLines(Path.ChangeExtension(Path.GetFileName(xmlOutPath), ".report"), result.Messages);

            Assert.True(result.Result, string.Join(Environment.NewLine, result.Messages));
        }

        [Theory]
        [InlineData("http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/finrep/its-005-2020/2022-06-01/mod/finrep9.json")]
        public static void ReadModuleDefinitionTest(string entrypoint)
        => ModuleDefinition.FromFile(entrypoint.Replace("http://", "taxonomy/"));

        [Theory]
        [InlineData("http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2022-06-01/mod/sbp_cr.json")]
        public static void DeserializeModuleFromJsonTest(string path)
        => ModuleDefinition.FromFile(path.Replace("http://", "taxonomy/"));

        [Theory]
        [InlineData("http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2022-06-01/tab/c_101.00/c_101.00.json")]
        public static void DeserializeTableFromJsonTest(string path)
        => TableDefinition.FromFile(path.Replace("http://", "taxonomy/"));

        public static string XmlToCsv(string reportPath)
        {
            var xmlReport = Xbrl.Xml.Report.FromFile(reportPath);

            var entrypoint = Path.ChangeExtension(xmlReport.SchemaReference.Value.Replace("http://", "taxonomy/"), "json");
            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var csvReport = xmlReport.ToXbrlCsv(moduleDefinition);

            var csvReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".zip");
            csvReport.Export(csvReportPath, moduleDefinition);
            return csvReportPath;
        }

        public static string CsvToXml(string reportPath)
        {

            var entrypoint = PlainCsvReport.GetPackageEntryPoint(reportPath).Replace(@"http://", "taxonomy/");

            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var csvReport = Report.FromFile(reportPath, moduleDefinition);

            var dimensionSpecification = "http://www.eba.europa.eu/eu/fr/xbrl/crr/dict/dim/4.2/dim.xsd";
            Dictionary<string, string> typedDimensions = new();
            //ReadTypedDimensions(dimensionSpecification.Replace("http://", "taxonomy/"));

            var typedDomainNamespace = KeyValuePair.Create("eba_typ", "http://www.eba.europa.eu/xbrl/crr/dict/typ");

            var xmlReport = csvReport.ToXbrlXml(typedDimensions, typedDomainNamespace, moduleDefinition);

            var xmlReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".xbrl");
            xmlReport.ToFile(xmlReportPath);
            return xmlReportPath;

        }

        // private static Dictionary<string, DimensionInfo> ReadDimensionInfo(string path)
        // {
        //     var result = new Dictionary<string, DimensionInfo>();

        //     XNamespace xs = "http://www.w3.org/2001/XMLSchema";
        //     XNamespace xbrldt = "http://xbrl.org/2005/xbrldt";

        //     XName element = xs + "element";
        //     XName typedDomainRef = xbrldt + "typedDomainRef";

        //     //xbrldt:typedDomainRef="../dom/typ.xsd#eba_SE"
        //     var document = XDocument.Load(path);
        //     var dimensions = document.Root.Elements(element).ToList();

        //     // TODO: Get the value from the actual definition instead of guessing 
        //     foreach (var dimension in dimensions)
        //     {
        //         var dimensionCode = dimension.Attribute("id");

        //         //var dimensionInfo = new DimensionInfo( dimension.Attribute("name").Value, 
        //     }

        //     // result.Add(
        //     //     typedDimensions.
        //     //         ToDictionary
        //     //         (
        //     //             dim => dim.Attribute("name").Value,
        //     //             dim => dim.Attribute(typedDomainRef).Value.Split('#').Last().Split('_').Last()
        //     //         );

        //     return result;
        // }

        // public static string CsvToXml(string reportPath)
        // {

        //     var entrypoint = PlainCsvReport.GetPackageEntryPoint(reportPath).Replace(@"http://", "taxonomy/");

        //     var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

        //     var csvReport = Report.FromFile(reportPath, moduleDefinition);

        //     var dimensionSpecification = "http://www.eba.europa.eu/eu/fr/xbrl/crr/dict/dim/4.2/dim.xsd";
        //     Dictionary<string, string> typedDimensions =
        //         ReadTypedDimensions(dimensionSpecification.Replace("http://", "taxonomy/"));

        //     var typedDomainNamespace = KeyValuePair.Create("eba_typ", "http://www.eba.europa.eu/xbrl/crr/dict/typ");

        //     var xmlReport = csvReport.ToXbrlXml(typedDimensions, typedDomainNamespace, moduleDefinition);

        //     var xmlReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".xbrl");
        //     xmlReport.ToFile(xmlReportPath);
        //     return xmlReportPath;

        // }

        // private static Dictionary<string, DimensionInfo> ReadDimensionInfo(string path)
        // {
        //     var result = new Dictionary<string, DimensionInfo>();

        //     XNamespace xs = "http://www.w3.org/2001/XMLSchema";
        //     XNamespace xbrldt = "http://xbrl.org/2005/xbrldt";

        //     XName element = xs + "element";
        //     XName typedDomainRef = xbrldt + "typedDomainRef";

        //     //xbrldt:typedDomainRef="../dom/typ.xsd#eba_SE"
        //     var document = XDocument.Load(path);
        //     var dimensions = document.Root.Elements(element).ToList();

        //     // TODO: Get the value from the actual definition instead of guessing 
        //     foreach (var dimension in dimensions)
        //     {
        //         var dimensionCode = dimension.Attribute("id");

        //         //var dimensionInfo = new DimensionInfo( dimension.Attribute("name").Value, 
        //     }

        //     // result.Add(
        //     //     typedDimensions.
        //     //         ToDictionary
        //     //         (
        //     //             dim => dim.Attribute("name").Value,
        //     //             dim => dim.Attribute(typedDomainRef).Value.Split('#').Last().Split('_').Last()
        //     //         );

        //     return result;
        // }


        [Theory]
        [InlineData("http://www.eba.europa.eu/eu/fr/xbrl/crr/dict/dim/4.2/dim-def.xml")]
        public static void ReadDimensionInfoTest(string path)
        {
            path = path.Replace("http://", "taxonomy/");
            ReadDimensionInfo(path);
        }

        private static void ReadDimensionInfo(string path)
        {
            XNamespace link = "http://www.xbrl.org/2003/linkbase";
            XName loc = link + "loc";
            XName definitionArc = link + "definitionArc";

            XNamespace xlink = "http://www.w3.org/1999/xlink";
            XName href = xlink + "href";
            XName label = xlink + "label";
            XName from = xlink + "from";
            XName to = xlink + "to";
            XName arcrole = xlink + "arcrole";

            var document = XDocument.Load(path);

            //  <link:loc xlink:type="locator" xlink:label="loc_dim_eba_qBXX" xlink:href="dim.xsd#eba_qBXX"/>
            var dimensions = document.Root.Descendants(loc).Where(l => l.Attribute(href).Value.StartsWith("dim.xsd#")).ToList();
            var dimensionDomainRole = "http://xbrl.org/int/dim/arcrole/dimension-domain";
            var result = new Dictionary<string, (string, bool)>();
            foreach (var dimension in dimensions)
            {

                var fromLabel = dimension.Attribute(label).Value;
                var dimensionref = dimension.Attribute(href).Value;
                var dimensionCode = dimensionref.Split('#').Last();

                //<link:loc xlink:type="locator" xlink:label="loc_dim_eba_qBRD" xlink:href="dim.xsd#eba_qBRD"/>
                //<link:definitionArc order="1005" xlink:arcrole="http://xbrl.org/int/dim/arcrole/dimension-domain" 
                //     xlink:from="loc_dim_eba_qBRD" xlink:to="loc_dom_eba_CU" xlink:type="arc" xbrldt:usable="false" />
                var toLabel = document.Root.
                    Descendants(definitionArc).
                    Where(d => d.Attribute(from).Value == fromLabel).
                    Single(d => d.Attribute(arcrole).Value == dimensionDomainRole).
                    Attribute(to).Value;

                //<link:loc xlink:type="locator" xlink:label="loc_dom_eba_GA" xlink:href="../../dom/exp.xsd#eba_GA"/>
                var domain = document.Root.
                    Descendants(loc).
                    Single(l => l.Attribute(label).Value == toLabel);

                var domainref = Path.GetFileName(domain.Attribute(href).Value).Split('#');
                var domaincode = domainref.Last();
                var typed = domainref.First() == "typ.xsd";
                result[dimensionCode] = (domaincode, typed);
            }
        }

        // public static HashSet<string> ReadTypedDomainInfo(string path)
        // => File.ReadAllLines(Path.Combine("data", "csv", path)).ToHashSet();

        // public static Dictionary<string, string> ReadDimensionDomainInfo(string file)
        // => File.ReadAllLines(Path.Combine("data", "csv", file)).
        //     Select(l => l.Split(',')).
        //     ToDictionary(x => x[0], x => x[1]);

    }
}
