namespace Diwen.XbrlCsv.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Diwen.Xbrl;
    using Diwen.Xbrl.Csv;
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
        public static void TaxonomyLookup()
        {
            var instance = new Instance();

            var packagePath = Path.Combine("csv", "DUMMYLEI123456789012.CON_FR_SBP010201_SBPCR_2022-12-31_20220411141758000.zip");
            var report = Report.Import(packagePath);
            var filed = report.FilingIndicators.Where(i => i.Value).Select(i => i.Key.ToLowerInvariant()).ToHashSet();

            var tabledata =
                report.
                Data.
                GroupBy(d => d.Table).
                Where(t => filed.Contains(t.Key)).
                ToDictionary(d => d.Key, d => d.ToArray());

            var rootpath = "/home/john/Downloads/EBA/EBA_CRD_XBRL_3.2_Reporting_Frameworks_3.2.2.0/";
            var entrypoint = Path.Combine(rootpath, report.Entrypoint.Replace(@"http://", ""));
            var modfolder = Path.GetDirectoryName(entrypoint);
            var taxfoder = Path.GetDirectoryName(modfolder);
            var tabfolder = Path.Combine(taxfoder, "tab");

            foreach (var table in tabledata)
            {
                var tabfile = Path.Combine(tabfolder, $"{table.Key}/{table.Key}.json");
                Console.Write(tabfile + "\t");
                Console.WriteLine(File.Exists(tabfile));
                using (var stream = new FileStream(tabfile, FileMode.Open, FileAccess.Read))
                using (var document = JsonDocument.Parse(stream))
                {
                    var root = document.RootElement;
                    var templates = root.GetProperty("tableTemplates");
                    var template = templates.GetProperty(table.Key.ToUpperInvariant().Replace('.', '-'));
                    var columns = template.GetProperty("columns");
                    var datapoint = columns.GetProperty("datapoint");
                    var propertyGroups = datapoint.GetProperty("propertyGroups");

                    foreach (var fact in table.Value)
                    {
                        var dp = propertyGroups.GetProperty(fact.Datapoint);
                        var dimensions = dp.GetProperty("dimensions");
                        string metric;
						var dimensionValues = new Dictionary<string,string>();
                        foreach (var prop in dimensions.EnumerateObject())
                        {
                            if (prop.Name == "concept")
                                metric = prop.Value.GetString();
							else
								dimensionValues.Add(prop.Name, prop.Value.GetString());
                            // var concept = dimensions.GetProperty("concept");
                            // var metric = concept.GetString();
                        }
						
                    }
                }
            }
        }
    }
}