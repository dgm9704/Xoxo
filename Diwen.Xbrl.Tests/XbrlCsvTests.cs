namespace Diwen.Xbrl.Csv.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Diwen.Xbrl;
    using Diwen.Xbrl.Comparison;
    using Diwen.Xbrl.Csv;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Diwen.Xbrl.Extensions;
    using Xunit;
    using Xunit.Abstractions;

    public class XbrlCsvTests
    {
        private readonly ITestOutputHelper output;

        public XbrlCsvTests(ITestOutputHelper output)
        => this.output = output;

        [Fact]
        public void ExportTests()
        {
            var report = new Report
            {
                Entrypoint = "http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2021-07-15/mod/sbp_cr_con.json",
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
            report.AddData("S_00.01", "dp31870", "eba_AS:x1");
            report.AddData("S_00.01", "dp37969", "eba_SC:x6");

            // datapoint,factValue,IRN
            report.AddData("C_105.03", "dp434188", "grarenmw", "IRN", "36");
            report.AddData("C_105.03", "dp434189", "eba_GA:AL", "IRN", "36");
            report.AddData("C_105.03", "dp434188", "grarenmw2", "IRN", "8");
            report.AddData("C_105.03", "dp434189", "eba_GA:AL", "IRN", "8");

            // datapoint,factValue,IMI,PBE
            report.AddData("C_105.02", "dp439585", "250238.28", ("IMI", "ksnpfnwn"), ("PBI", "ksnpfnwn"));
            report.AddData("C_105.02", "dp439586", "247370.72", ("IMI", "ksnpfnwn"), ("PBI", "ksnpfnwn"));
            report.AddData("C_105.02", "dp439585", "250238.28", ("IMI", "kotnyngp"), ("PBI", "kotnyngp"));
            report.AddData("C_105.02", "dp439586", "247370.72", ("IMI", "kotnyngp"), ("PBI", "kotnyngp"));


            // datapoint,factValue,FTY,INC
            report.AddData("C_113.00", "dp439732", "304132.94", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439750", "eba_IM:x33", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439744", "0.1", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439745", "0.72", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439751", "0.34", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439752", "0.46", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439753", "eba_ZZ:x409", ("FTY", "htkaaxvr"), ("INC", "htkaaxvr"));
            report.AddData("C_113.00", "dp439732", "304132.94", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113.00", "dp439750", "eba_IM:x33", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113.00", "dp439744", "0.1", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113.00", "dp439745", "0.72", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113.00", "dp439751", "0.34", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113.00", "dp439752", "0.46", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));
            report.AddData("C_113.00", "dp439753", "eba_ZZ:x409", ("FTY", "ynqtbutq"), ("INC", "ynqtbutq"));

            report.Export("DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000");
        }

        [Theory]
        [InlineData("DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000.zip")]
        public static void ReadPackageTest(string packageName)
        {
            var packagePath = Path.Combine("csv", packageName);
            var reportFiles = Report.ReadPackage(packagePath);

            var metafolder = "META-INF";
            var reportfolder = "reports";

            Assert.True(reportFiles.ContainsKey(Path.Combine(metafolder, "reports.json")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "report.json")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "parameters.csv")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "FilingIndicators.csv")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "S_00.01.csv")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "C_105.02.csv")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "C_105.03.csv")));
            Assert.True(reportFiles.ContainsKey(Path.Combine(reportfolder, "C_113.00.csv")));
        }

        [Theory]
        [InlineData("DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000.zip")]
        public static void ImportTest(string packageName)
        {
            var packagePath = Path.Combine("csv", packageName);
            var report = Report.FromFile(packagePath);

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
        [InlineData("csv/DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.xbrl")]
        [InlineData("csv/F_18-00-a.xbrl")]
        public void XmlToCsvTest(string reportPath)
        => XmlToCsv(reportPath);

        [Theory]
        [InlineData("csv/DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.zip")]
        [InlineData("csv/F_18-00-a.zip")]
        public void CsvToXmlTest(string reportPath)
        => CsvToXml(reportPath);

        [Theory]
        [InlineData("csv/DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.xbrl")]
        [InlineData("csv/FINREP_F_23-01_R0080_C0010.xbrl")]
        [InlineData("csv/FINREP_F_40-01_R999_C0031.xbrl")]
        public void XmlToCsvToXml(string xmlInPath)
        {
            var csvPath = XmlToCsv(xmlInPath);
            var xmlOutPath = CsvToXml(csvPath);
            var result = InstanceComparer.Report(xmlInPath, xmlOutPath);
            if (!result.Result)
                File.WriteAllLines(Path.ChangeExtension(Path.GetFileName(xmlOutPath), ".report"), result.Messages);

            Assert.True(result.Result, string.Join(Environment.NewLine, result.Messages));
        }

        [Theory]
        [InlineData("EBA32_TypedDomain.csv")]
        public void ReadTypedDomainInfoTest(string path)
        => ReadTypedDomainInfo(path);

        [Theory]
        [InlineData("EBA32_finrep_FilingIndicators.csv")]
        public void ReadFilingIndicatorInfoTest(string file)
        => ReadFilingIndicatorInfo(file);

        public static Dictionary<string, string> ReadFilingIndicatorInfo(string file)
        => File.ReadAllLines(Path.Combine("csv", file)).
            Select(l => l.Split(',')).
            ToDictionary(x => x[0], x => x[1]);

        [Theory]
        [InlineData("www.eba.europa.eu/eu/fr/xbrl/crr/fws/finrep/its-005-2020/2022-06-01/mod/finrep9.json")]
        public static void ReadModuleDefinitionTest(string entrypoint)
        => ModuleDefinition.FromFile(entrypoint);

        [Theory]
        [InlineData("EBA32_DimensionDomain.csv")]
        public static void ReadDimensionDomainInfoTest(string file)
        => ReadDimensionDomainInfo(file);

        [Theory]
        [InlineData("www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2022-06-01/mod/sbp_cr.json")]
        public static void DeserializeModuleFromJsonTest(string path)
        => ModuleDefinition.FromFile(path);

        [Theory]
        [InlineData("www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2022-06-01/tab/c_101.00/c_101.00.json")]
        public static void DeserializeTableFromJsonTest(string path)
        => TableDefinition.FromFile(path);

        public string XmlToCsv(string reportPath)
        {
            var xmlReport = Instance.FromFile(reportPath);

            var entrypoint = Path.ChangeExtension(xmlReport.SchemaReference.Value.Replace("http://", ""), "json");
            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var tableDefinitions = moduleDefinition.TableDefinitions();

            var filingIndicators = ReadFilingIndicatorInfo("EBA32_finrep_FilingIndicators.csv");

            var csvReport = xmlReport.ToXbrlCsv(tableDefinitions, filingIndicators, moduleDefinition);

            var csvReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".zip");
            csvReport.Export(csvReportPath);
            return csvReportPath;
        }

        public string CsvToXml(string reportPath)
        {

            var csvReport = Report.FromFile(reportPath);

            var entrypoint = csvReport.Entrypoint.Replace(@"http://", "");

            var moduleDefinition = ModuleDefinition.FromFile(entrypoint);

            var tableDefinitions = moduleDefinition.TableDefinitions();

            var dimensionDomainInfo = ReadDimensionDomainInfo("EBA32_DimensionDomain.csv");

            var typedDomains = ReadTypedDomainInfo("EBA32_TypedDomain.csv");

            var typedDomainNamespace = KeyValuePair.Create("eba_typ", "http://www.eba.europa.eu/xbrl/crr/dict/typ");

            var filingIndicators = ReadFilingIndicatorInfo("EBA32_finrep_FilingIndicators.csv");

            var xmlReport = csvReport.ToXbrlXml(tableDefinitions, dimensionDomainInfo, typedDomainNamespace, filingIndicators, typedDomains, moduleDefinition);

            var xmlReportPath = Path.ChangeExtension(Path.GetFileName(reportPath), ".xbrl");
            xmlReport.ToFile(xmlReportPath);
            return xmlReportPath;

        }

        public static HashSet<string> ReadTypedDomainInfo(string path)
        => File.ReadAllLines(Path.Combine("csv", path)).ToHashSet();

        public static Dictionary<string, string> ReadDimensionDomainInfo(string file)
        => File.ReadAllLines(Path.Combine("csv", file)).
            Select(l => l.Split(',')).
            ToDictionary(x => x[0], x => x[1]);

    }
}