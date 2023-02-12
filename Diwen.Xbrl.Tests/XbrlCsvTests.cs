namespace Diwen.XbrlCsv.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Diwen.Xbrl;
    using Diwen.Xbrl.Csv;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Xunit;

    public class XbrlCsvTests
    {
        [Fact]
        public void ExportTests()
        {
            var report = new Report();
            report.Entrypoint = "http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2021-07-15/mod/sbp_cr_con.json";

            report.Parameters.Add("entityID", "lei:DUMMYLEI123456789012");
            report.Parameters.Add("refPeriod", "2021-12-31");
            report.Parameters.Add("baseCurrency", "iso4217:EUR");
            report.Parameters.Add("decimalsInteger", "0");
            report.Parameters.Add("decimalsMonetary", "-3");
            report.Parameters.Add("decimalsPercentage", "4");
            report.Parameters.Add("decimalsDecimal", "2");

            report.FilingIndicators.Add("C_101.00", false);
            report.FilingIndicators.Add("C_102.00", false);
            report.FilingIndicators.Add("C_103.00", false);
            report.FilingIndicators.Add("C_105.01", false);
            report.FilingIndicators.Add("C_105.02", true);
            report.FilingIndicators.Add("C_105.03", true);
            report.FilingIndicators.Add("C_111.00", false);
            report.FilingIndicators.Add("C_112.00", false);
            report.FilingIndicators.Add("C_113.00", true);
            report.FilingIndicators.Add("S_00.01", true);

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

        [Fact]
        public static void ReadPackageTest()
        {
            var packagePath = Path.Combine("csv", "DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000.zip");
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

        [Fact]
        public static void ImportTest()
        {
            var packagePath = Path.Combine("csv", "DUMMYLEI123456789012_GB_SBP010200_SBPCRCON_2021-12-31_20210623163233000.zip");
            var report = Report.Import(packagePath);

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

        [Fact]
        public static void XbrlCsvToXml()
        {
            var packagePath = Path.Combine("csv", "DUMMYLEI123456789012.CON_FR_SBP010201_SBPIFRS9_2022-12-31_20220411141759000.zip");

            var report = Report.Import(packagePath);

            var rootpath = "";
            var entrypoint = Path.Combine(rootpath, report.Entrypoint.Replace(@"http://", ""));
            var modfolder = Path.GetDirectoryName(entrypoint);

            JsonModule module;
            using (var stream = new FileStream(entrypoint, FileMode.Open, FileAccess.Read))
                module = (JsonModule)JsonSerializer.Deserialize(stream, typeof(JsonModule));

            var jsonTables = new Dictionary<string, JsonTable>();

            var dimensionDomain = ReadDimensionDomainInfo();

            foreach (var moduleTable in module.documentInfo.extends)
            {
                var tabfile = Path.GetFullPath(Path.Combine(modfolder, moduleTable));
                if (File.Exists(tabfile))
                    using (var stream = new FileStream(tabfile, FileMode.Open, FileAccess.Read))
                    {
                        var jsonTable = (JsonTable)JsonSerializer.Deserialize(stream, typeof(JsonTable));
                        var tablecode = jsonTable.tableTemplates.Single().Key;
                        jsonTables.Add(tablecode, jsonTable);
                    }
            }

            var instance = report.ToXml(jsonTables, dimensionDomain);

            instance.ToFile(Path.ChangeExtension(packagePath, ".xbrl"));
        }

        [Fact]
        public static void XbrlCsvToXml2()
        {

            var packagePath = Path.Combine("csv", "DUMMYLEI123456789012.CON_FR_FINREP030100_FINREP9_2022-12-31_20220411141600000.zip");
            var report = Report.Import(packagePath);

            var rootpath = "";
            var entrypoint = Path.Combine(rootpath, report.Entrypoint.Replace(@"http://", ""));
            var modfolder = Path.GetDirectoryName(entrypoint);

            JsonModule module;
            using (var stream = new FileStream(entrypoint, FileMode.Open, FileAccess.Read))
                module = (JsonModule)JsonSerializer.Deserialize(stream, typeof(JsonModule));

            var jsonTables = new Dictionary<string, JsonTable>();

            var dimensionDomain = ReadDimensionDomainInfo();

            foreach (var moduleTable in module.documentInfo.extends)
            {
                var tabfile = Path.GetFullPath(Path.Combine(modfolder, moduleTable));
                if (File.Exists(tabfile))
                    using (var stream = new FileStream(tabfile, FileMode.Open, FileAccess.Read))
                    {
                        var jsonTable = (JsonTable)JsonSerializer.Deserialize(stream, typeof(JsonTable));
                        var tablecode = jsonTable.tableTemplates.Single().Key;
                        jsonTables.Add(tablecode, jsonTable);
                    }
            }

            var instance = report.ToXml(jsonTables, dimensionDomain);

            instance.ToFile(Path.ChangeExtension(packagePath, ".xbrl"));
        }

        private static Dictionary<string, string> ReadDimensionDomainInfo()
        => File.ReadAllLines(Path.Combine("csv", "EBA32_DimensionDomain.csv")).
            Select(l => l.Split(',')).
            ToDictionary(x => x[0], x => x[1]);

        [Fact]
        public static void DeserializeModuleFromJson()
        {
            var path = "www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2022-06-01/mod/sbp_cr.json";
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var module = JsonSerializer.Deserialize(stream, typeof(JsonModule));
                Assert.NotNull(module);
            }
        }

        [Fact]
        public static void DeserializeTableFromJson()
        {
            var path = "www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2022-06-01/tab/c_101.00/c_101.00.json";
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var table = JsonSerializer.Deserialize(stream, typeof(JsonTable));
                Assert.NotNull(table);
            }
        }

    }
}