namespace Diwen.Xbrl.Csv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Diwen.Xbrl.Extensions;

    public class Report
    {
        public string DocumentType { get; set; } = "https://xbrl.org/CR/2021-02-03/xbrl-csv";

        public string Entrypoint { get; set; }

        public Dictionary<string, bool> FilingIndicators = new Dictionary<string, bool>();

        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public List<ReportData> Data = new List<ReportData>();

        public void AddData(string table, string datapoint, string value)
        => Data.Add(new ReportData(table, datapoint, value));

        public void AddData(string table, string datapoint, string value, params (string key, string value)[] pairs)
        => Data.Add(new ReportData(table, datapoint, value, pairs));

        public void AddData(string table, string datapoint, string value, string dimensionKey, string dimensionValue)
        => Data.Add(new ReportData(table, datapoint, value));

        public void AddData(string table, string datapoint, string value, Dictionary<string, string> dimensions)
        => Data.Add(new ReportData(table, datapoint, value, dimensions));

        public void Export(string packagename)
        {
            var package = CreatePackage();
            var zip = CreateZip(package, packagename);
            WriteStreamToFile(zip, Path.ChangeExtension(packagename, "zip"));
        }

        private Dictionary<string, Stream> CreatePackage()
        {
            var metafolder = "META-INF";
            var reportfolder = "reports";
            var package = new Dictionary<string, Stream>();

            package.Add(Path.Combine(metafolder, "reports.json"), CreatePackageInfo());
            package.Add(Path.Combine(reportfolder, "report.json"), CreateReportInfo());
            package.Add(Path.Combine(reportfolder, "parameters.csv"), CreateParameters());
            package.Add(Path.Combine(reportfolder, "FilingIndicators.csv"), CreateFilingIndicators());
            foreach ((string path, Stream stream) in CreateReportData(reportfolder))
                package.Add(path, stream);

            return package;
        }

        private void WriteStreamToFile(Stream stream, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                stream.CopyTo(fileStream);
        }

        private Stream CreateZip(Dictionary<string, Stream> package, string packagename)
        {
            var stream = new MemoryStream();
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                foreach (var item in package)
                {
                    ZipArchiveEntry entry = zip.CreateEntry(item.Key, CompressionLevel.Optimal);
                    using (var entryStream = entry.Open())
                        item.Value.CopyTo(entryStream);
                }
            }
            stream.Flush();
            stream.Position = 0;
            return stream;
        }

        private Stream CreatePackageInfo()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("{\"documentInfo\":{\"documentType\":\"http://xbrl.org/PWD/2020-12-09/report-package\"}}");
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private Stream CreateReportInfo()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write($"{{\"documentInfo\":{{\"documentType\":\"{DocumentType}}}\",\"extends\":[\"{Entrypoint}\"]}}}}");
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private Stream CreateParameters()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            var builder = new StringBuilder();
            builder.AppendLine("name,value");
            foreach (var p in Parameters)
                builder.AppendLine($"{p.Key},{p.Value}");
            writer.Write(builder.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private Stream CreateFilingIndicators()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            var builder = new StringBuilder();
            builder.AppendLine("templateId,reported");
            foreach (var fi in FilingIndicators)
                builder.AppendLine($"{fi.Key},{fi.Value.ToString().ToLower()}");
            writer.Write(builder.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private List<(string, Stream)> CreateReportData(string folderpath)
        {
            var reportdata = new List<(string, Stream)>();

            foreach (var template in FilingIndicators.Where(fi => fi.Value))
            {
                var tabledata = Data.Where(d => d.Table == template.Key);
                if (tabledata.Any())
                {
                    var filename = template.Key + ".csv";
                    var filepath = Path.Combine(folderpath, filename);
                    var builder = new StringBuilder("datapoint,factvalue");
                    foreach (var dimension in tabledata.First().Dimensions.Keys)
                        builder.Append($",{dimension}");
                    builder.AppendLine();

                    foreach (var item in tabledata)
                    {
                        builder.AppendFormat($"{item.Datapoint},{item.Value}");
                        foreach (var dimension in item.Dimensions.Values)
                            builder.Append($",{dimension}");
                        builder.AppendLine();
                    }
                    var stream = new MemoryStream();
                    var writer = new StreamWriter(stream);
                    writer.Write(builder.ToString());
                    writer.Flush();
                    stream.Position = 0;
                    reportdata.Add((filepath, stream));
                }
            }
            return reportdata;
        }

        public static Report Import(string packagePath)
        {
            var report = new Report();
            var reportFiles = ReadPackage(packagePath);
            var packagename = Path.GetFileNameWithoutExtension(packagePath);

            report.Entrypoint = ReadEntryPoint(reportFiles.Single(f => f.Key.EndsWith("reports/report.json")).Value);
            report.Parameters = ReadParameters(reportFiles.Single(f => f.Key.EndsWith("reports/parameters.csv")).Value);
            report.FilingIndicators = ReadFilingIndicators(reportFiles.Single(f => f.Key.EndsWith("reports/FilingIndicators.csv")).Value);
            foreach (var template in report.FilingIndicators.Where(fi => fi.Value).Select(fi => fi.Key))
                foreach (var tablefile in reportFiles.Where(f => Path.GetFileNameWithoutExtension(f.Key).StartsWith(template, StringComparison.OrdinalIgnoreCase)))
                    report.Data.AddRange(ReadTableData(Path.GetFileNameWithoutExtension(tablefile.Key), tablefile.Value));

            return report;
        }

        private static string ReadEntryPoint(string data)
        {
            var expression = new Regex(@"[^\""]*\.json", RegexOptions.Compiled);
            var match = expression.Match(data);
            return match.Value;
        }

        private static IEnumerable<ReportData> ReadTableData(string table, string data)
        {
            var result = new List<ReportData>();
            var records =
                data.
                Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).
                Select(line => line.Split(','));

            var header = records.First();
            foreach (var record in records.Skip(1))
            {
                var datapoint = record[0];
                var value = record[1];
                var item = new ReportData(table, datapoint, value);
                for (int i = 2; i < header.Length; i++)
                    item.Dimensions.Add(header[i], record[i]);

                result.Add(item);
            }
            return result;
        }

        private static Dictionary<string, bool> ReadFilingIndicators(string data)
        => data.
            Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).
            Skip(1).
            Select(line => line.Split(',')).
            ToDictionary(
                f => f[0],
                f => Convert.ToBoolean(f[1]));

        private static Dictionary<string, string> ReadParameters(string data)
        => data.
            Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).
            Skip(1).
            Select(line => line.Split(',')).
            ToDictionary(
                p => p[0],
                p => p[1]);

        public static Dictionary<string, string> ReadPackage(string packagePath)
        {
            var reportFiles = new Dictionary<string, string>();
            using (var packageStream = File.OpenRead(packagePath))
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    using (var entryStream = entry.Open())
                    using (var memoryStream = new MemoryStream())
                    {
                        entryStream.CopyTo(memoryStream);
                        string content = Encoding.UTF8.GetString(memoryStream.ToArray());
                        reportFiles.Add(entry.FullName, content);
                    }
                }
            }
            return reportFiles;
        }

        public Instance ToXml(Dictionary<string, TableDefinition> tableDefinitions, Dictionary<string, string> dimensionDomain, KeyValuePair<string, string> typedDomainNamespace)
        => ToXml(this, tableDefinitions, dimensionDomain, typedDomainNamespace);

        public static Instance ToXml(Report report, Dictionary<string, TableDefinition> tableDefinitions, Dictionary<string, string> dimensionDomain, KeyValuePair<string, string> typedDomainNamespace)
        {
            var instance = new Instance();
            var baseCurrency = report.Parameters["baseCurrency"];
            var baseCurrencyRef = $"u{baseCurrency.Split(':').Last()}";
            instance.Units.Add(baseCurrencyRef, baseCurrency);
            instance.SetTypedDomainNamespace(typedDomainNamespace.Key, typedDomainNamespace.Value);

            var filed = report.FilingIndicators.Where(i => i.Value).Select(i => i.Key.ToLowerInvariant()).ToHashSet();

            var tabledata =
                report.
                Data.
                GroupBy(d => d.Table).
                Where(t => filed.Contains(t.Key)).
                ToDictionary(d => d.Key, d => d.ToArray());


            foreach (var table in tabledata)
            {
                var tablecode = table.Key.ToUpperInvariant().Replace('.', '-');
                var jsonTable = tableDefinitions[tablecode];

                foreach (var ns in jsonTable.documentInfo.namespaces)
                {
                    if (ns.Key.EndsWith("_dim"))
                        instance.SetDimensionNamespace(ns.Key, ns.Value);
                    else if (ns.Key.EndsWith("_met"))
                        instance.SetMetricNamespace(ns.Key, ns.Value);
                    else
                        instance.AddDomainNamespace(ns.Key, ns.Value);
                }

                var propertyGroups = jsonTable.tableTemplates[tablecode].columns.datapoint.propertyGroups;
                foreach (var fact in table.Value)
                {
                    var scenario = new Scenario();
                    var dimensions = propertyGroups[fact.Datapoint].dimensions;
                    string metric = string.Empty;
                    string unit = string.Empty;

                    foreach (var dimension in dimensions)
                    {
                        if (dimension.Key == "concept")
                            metric = dimension.Value.Split(':').Last();
                        else if (dimension.Key == "unit")
                            unit = dimension.Value;
                        else
                            scenario.AddExplicitMember(dimension.Key, dimension.Value);
                    }

                    foreach (var d in fact.Dimensions)
                        scenario.AddTypedMember(d.Key, dimensionDomain[d.Key], d.Value);

                    var unitRef = unit.Replace("$baseCurrency", baseCurrencyRef);

                    instance.AddFact(scenario, metric, unitRef, "", fact.Value);

                }

            }
            return instance;

        }

        public static Report FromXml(Instance xmlReport, Dictionary<string, TableDefinition> tableDefinitions)
        {
            var report = new Report();

            report.Entrypoint = Path.ChangeExtension(xmlReport.SchemaReference.Value, ".json");

            report.Parameters.Add("entityID", xmlReport.Entity.Identifier.Value);
            report.Parameters.Add("refPeriod", xmlReport.Period.Instant.ToString("yyyy-MM-dd"));
            report.Parameters.Add("baseCurrency", xmlReport.Units.First(u => u.Measure.Namespace == "http://www.xbrl.org/2003/iso4217").Measure.LocalName());
            report.Parameters.Add("decimalsInteger", "0");
            report.Parameters.Add("decimalsMonetary", "-3");
            report.Parameters.Add("decimalsPercentage", "4");
            report.Parameters.Add("decimalsDecimal", "2");

            foreach (var fi in xmlReport.FilingIndicators)
                report.FilingIndicators.Add(fi.Value, fi.Filed);

            var dimNsPrefix = xmlReport.Namespaces.LookupPrefix(
                xmlReport.Contexts.First(c => c.Scenario != null && c.Scenario.ExplicitMembers.Any()).
                Scenario.ExplicitMembers.First().Dimension.Namespace);

            var reportedTables =
                tableDefinitions.
                Where(td => report.FilingIndicators[td.Key.Replace('-', '.')]).
                ToDictionary(t => t.Key, t => t.Value);

            foreach (var fact in xmlReport.Facts)
            {
                var value = fact.Value;
                var scenario = fact.Context.Scenario;
                var datapoint = GetDatapoint(fact, reportedTables, dimNsPrefix);
            }


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

            return report;
        }

        // "tableTemplates": {
        //         "S_00-01": {
        //             "columns": {
        //                 "datapoint": {
        //                     "propertyGroups": {
        //                         "dp31870": {
        //                             "dimensions": {
        //                                 "concept": "eba_met:ei4",
        //                                 "eba_dim:BAS": "eba_BA:x17"
        //                             },
        //                             "eba:documentation": {
        //                                 "CellCode": "{S 00.01, r0010, c0010}",
        //                                 "DataPointVersionId": "31870"
        //                             }
        //                         },

        private static string GetDatapoint(Fact fact, Dictionary<string, TableDefinition> tabledefinitions, string dimNsPrefix)
        {
            var metric = fact.Metric.Name;

            foreach (var td in tabledefinitions)
            {
                // filter by metric
                var candidateDatapoints =
                td.Value.tableTemplates.First().Value.
                    columns.
                        datapoint.
                            propertyGroups.
                                Where(pg => pg.Value.dimensions["concept"] == metric).
                                ToArray();

                // filter by matching explicit members
                candidateDatapoints =
                candidateDatapoints.
                    Where(pg => fact.Context.Scenario.ExplicitMembers.
                    All(m => pg.Value.dimensions.GetValueOrDefault($"{dimNsPrefix}:{m.Dimension.Name}", string.Empty) == m.MemberCode))
                    .ToArray();

                if(candidateDatapoints.Any())
                    return candidateDatapoints.First().Key;
            }

            return string.Empty;
        }
    }
}