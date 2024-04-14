namespace Diwen.Xbrl.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Diwen.Xbrl.Extensions;
    using Diwen.Xbrl.Package;

    public partial class Report
    {
        public string DocumentType { get; set; } = "https://xbrl.org/CR/2021-02-03/xbrl-csv";

        public string Entrypoint { get; set; }

        public Dictionary<string, bool> FilingIndicators = [];

        public Dictionary<string, string> Parameters = [];

        public List<ReportData> Data = [];

        public void AddData(string table, string datapoint, string value)
        => Data.Add(new ReportData(table, datapoint, value));

        public void AddData(string table, string datapoint, string value, params (string key, string value)[] pairs)
        => Data.Add(new ReportData(table, datapoint, value, pairs));

        public void AddData(string table, string datapoint, string value, string dimensionKey, string dimensionValue)
        => Data.Add(new ReportData(table, datapoint, value));

        public void AddData(string table, string datapoint, string value, Dictionary<string, string> dimensions)
        => Data.Add(new ReportData(table, datapoint, value, dimensions));

        public void Export(string packagePath)
        {
            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            var package = CreatePackage(packageName, DocumentType, Entrypoint, Parameters, FilingIndicators, Data);
            var zip = CreateZip(package);
            WriteStreamToFile(zip, Path.ChangeExtension(packagePath, "zip"));
        }

        private static Dictionary<string, Stream> CreatePackage(string packageName, string documentType, string entrypoint, Dictionary<string, string> parameters, Dictionary<string, bool> filingIndicators, List<ReportData> data)
        {
            var metafolder = "META-INF";
            var reportfolder = "reports";

            var package = new Dictionary<string, Stream>
            {
                [Path.Combine(packageName, metafolder, "reports.json")] = CreatePackageInfo(),
                [Path.Combine(packageName, reportfolder, "report.json")] = CreateReportInfo(documentType, entrypoint),
                [Path.Combine(packageName, reportfolder, "parameters.csv")] = CreateParameters(parameters),
                [Path.Combine(packageName, reportfolder, "FilingIndicators.csv")] = CreateFilingIndicators(filingIndicators),
            };

            foreach (var tableStream in CreateReportData(data))
                package.Add(Path.Combine(packageName, reportfolder, tableStream.Key), tableStream.Value);

            return package;
        }

        private static void WriteStreamToFile(Stream stream, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                stream.CopyTo(fileStream);
        }

        private static Stream CreateZip(Dictionary<string, Stream> package)
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

        private static Stream CreatePackageInfo()
        {
            var info = new PackageInfo
            {
                documentInfo = new DocumentInfo
                {
                    documentType = "http://xbrl.org/PWD/2020-12-09/report-package"
                }
            };

            var stream = new MemoryStream();
            JsonSerializer.Serialize<PackageInfo>(stream, info);
            stream.Position = 0;
            return stream;
        }

        private static Stream CreateReportInfo(string documentType, string entrypoint)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var version = assembly.Version;
            var assemblyName = assembly.Name;
            var compileTime = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);

            var documentInfo = new DocumentInfo
            {
                documentType = documentType,
                extends = new List<string> { entrypoint }
            };

            var softwareInfo = new EbaGeneratingSoftwareInformation
            {
                ebasoftwareId = assemblyName,
                ebasoftwareVersion = version.ToString(),
                ebasoftwareCreationDate = $"{compileTime.Date:yyyy-MM-dd}",
                ebasoftwareAdditionalInfo = "https://github.com/dgm9704/Xoxo"
            };

            var stream = new MemoryStream();
            var reportInfo = new ReportInfo
            {
                documentInfo = documentInfo,
                ebageneratingSoftwareInformation = softwareInfo
            };

            JsonSerializer.Serialize<ReportInfo>(stream, reportInfo);
            stream.Position = 0;
            return stream;
        }


        private static Stream CreateParameters(Dictionary<string, string> parameters)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            var builder = new StringBuilder();
            builder.AppendLine("name,value");
            foreach (var p in parameters)
                builder.AppendLine($"{p.Key},{p.Value}");
            writer.Write(builder.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static Stream CreateFilingIndicators(Dictionary<string, bool> filingIndicators)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            var builder = new StringBuilder();
            builder.AppendLine("templateId,reported");
            foreach (var fi in filingIndicators)
                builder.AppendLine($"{fi.Key},{fi.Value.ToString().ToLower()}");
            writer.Write(builder.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static Dictionary<string, Stream> CreateReportData(List<ReportData> data)
        {
            var reportdata = new Dictionary<string, Stream>();

            var tabledata = data.GroupBy(d => d.Table);
            foreach (var table in tabledata)
            {
                var filename = table.Key + ".csv";
                var builder = new StringBuilder("datapoint,factvalue");
                foreach (var dimension in table.First().Dimensions.Keys)
                    builder.Append($",{dimension}");
                builder.AppendLine();

                foreach (var item in table)
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
                reportdata[filename] = stream;
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
            var reportInfo = JsonSerializer.Deserialize<ReportInfo>(data);
            return reportInfo.documentInfo.extends.First();
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
            Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).
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

        public Instance ToXbrlXml(Dictionary<string, TableDefinition> tableDefinitions, Dictionary<string, string> dimensionDomain, KeyValuePair<string, string> typedDomainNamespace, Dictionary<string, string> filingIndicators, HashSet<string> typedDomains, ModuleDefinition moduleDefinition)
        => ToXbrlXml(this, tableDefinitions, dimensionDomain, typedDomainNamespace, filingIndicators, typedDomains, moduleDefinition);

        public static Instance ToXbrlXml(
            Report report,
            Dictionary<string, TableDefinition> tableDefinitions,
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            Dictionary<string, string> filingIndicators,
            HashSet<string> typedDomains,
            ModuleDefinition moduleDefinition)
        {
            var instance = new Instance
            {
                SchemaReference = new SchemaReference("simple", moduleDefinition.DocumentInfo.taxonomy.FirstOrDefault())
            };

            foreach (var ns in moduleDefinition.DocumentInfo.namespaces)
                instance.Namespaces.AddNamespace(ns.Key, ns.Value);

            var idParts = report.Parameters["entityID"].Split(':');
            var idNs = instance.Namespaces.LookupNamespace(idParts.First());
            instance.Entity = new Entity(idNs, idParts.Last());

            instance.Period = new Period(DateTime.ParseExact(report.Parameters["refPeriod"], "yyyy-MM-dd", CultureInfo.InvariantCulture));

            var baseCurrency = report.Parameters["baseCurrency"];
            var baseCurrencyRef = $"u{baseCurrency.Split(':').Last()}";
            instance.Units.Add(baseCurrencyRef, $"iso4217:{baseCurrency}");
            instance.Units.Add("uPURE", "xbrli:pure");

            instance.SetTypedDomainNamespace(typedDomainNamespace.Key, typedDomainNamespace.Value);

            foreach (var fi in report.FilingIndicators)
                instance.AddFilingIndicator(fi.Key, fi.Value);

            var filed =
                report.
                FilingIndicators.
                Where(i => i.Value).
                Select(i => i.Key).
                ToHashSet();

            var tabledata =
                report.
                Data.
                Where(d => !string.IsNullOrEmpty(d.Value)).
                Where(d => filed.Contains(filingIndicators[d.Table])).
                GroupBy(d => d.Table).
                ToDictionary(d => d.Key, d => d.ToArray());

            var usedContexts = new Dictionary<string, Context>();
            var usedDatapoints = new HashSet<string>();

            foreach (var table in tabledata)
            {
                // var sw = Stopwatch.StartNew();
                var tableDefinition = tableDefinitions[table.Key];
                AddFactsForTable(report.Parameters, tableDefinition, dimensionDomain, typedDomainNamespace, typedDomains, instance, baseCurrencyRef, table, usedContexts, usedDatapoints);
                // sw.Stop();
                // Console.WriteLine($"AddFactsForTable {table.Key} {sw.Elapsed}");
            }

            instance.RemoveUnusedUnits();

            return instance;

        }

        private static string AddFactsForTable(
            Dictionary<string, string> parameters,
            TableDefinition tableDefinition,
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            HashSet<string> typedDomains,
            Instance instance,
            string baseCurrencyRef,
            KeyValuePair<string, ReportData[]> table,
            Dictionary<string, Context> usedContexts,
            HashSet<string> usedDatapoints)
        {
            string dimensionPrefix = string.Empty;

            foreach (var ns in tableDefinition.DocumentInfo.namespaces)
            {
                if (ns.Key.EndsWith("_dim"))
                {
                    dimensionPrefix = ns.Key;
                    instance.SetDimensionNamespace(ns.Key, ns.Value);
                }
                else if (ns.Key.EndsWith("_met"))
                {
                    instance.SetMetricNamespace(ns.Key, ns.Value);
                }
                else
                {
                    instance.AddDomainNamespace(ns.Key, ns.Value);
                }
            }

            foreach (var fact in table.Value)
            {
                var datapoint = tableDefinition.Datapoints[fact.Datapoint];
                AddFact(parameters, dimensionDomain, typedDomainNamespace, typedDomains, instance, baseCurrencyRef, dimensionPrefix, datapoint, fact, usedContexts, usedDatapoints);
            }
            return dimensionPrefix;
        }

        private static void AddFact(
            Dictionary<string, string> parameters,
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            HashSet<string> typedDomains,
            Instance instance,
            string baseCurrencyRef,
            string dimensionPrefix,
            PropertyGroup datapoint,
            ReportData fact,
            Dictionary<string, Context> usedContexts,
            HashSet<string> usedDatapoints)
        {
            var scenario = new Scenario(instance);
            string metric = string.Empty;
            string unit = string.Empty;

            foreach (var dimension in datapoint.Dimensions)
            {
                switch (dimension.Key)
                {
                    case "concept":
                        metric = dimension.Value.Split(':').Last();
                        break;

                    case "unit":
                        unit = dimension.Value;
                        break;

                    default:
                        scenario.AddExplicitMember(dimension.Key, dimension.Value);
                        break;
                }
            }

            foreach (var d in fact.Dimensions)
                if (typedDomains.Contains(dimensionDomain[d.Key]))
                    // HACK: For some reason the prefixes aren't found during normal operation so we have to add them here
                    // Find why the prefixs are dropped and remove this
                    scenario.AddTypedMember($"{dimensionPrefix}:{d.Key}", $"{typedDomainNamespace.Key}:{dimensionDomain[d.Key]}", d.Value);
                else
                    scenario.AddExplicitMember(d.Key, d.Value);

            var decimals =
                !string.IsNullOrEmpty(datapoint.Decimals)
                    ? parameters.GetValueOrDefault(datapoint.Decimals.TrimStart('$'), string.Empty)
                    : string.Empty;

            // Unit for only numeric values, ie. those that have decimals specified
            var unitRef =
                string.IsNullOrEmpty(decimals)
                    ? string.Empty
                    : !string.IsNullOrEmpty(unit)
                        ? unit.Replace("$baseCurrency", baseCurrencyRef)
                        : "uPURE";

            // scenario.Instance = instance;
            var scenarioKey = scenario.ToString();
            var datapointKey = $"{scenarioKey}+{metric}";
            if (usedContexts.TryGetValue(scenarioKey, out Context context))
            {
                if (!usedDatapoints.Contains(datapointKey))
                {
                    instance.AddFact(context, metric, unitRef, decimals, fact.Value);
                    usedDatapoints.Add(datapointKey);
                }
            }
            else
            {
                context = instance.CreateContext(scenario);
                usedContexts[scenarioKey] = context;
                instance.AddFact(context, metric, unitRef, decimals, fact.Value);
                usedDatapoints.Add(datapointKey);
            }
        }

        public static Report FromXml(Instance xmlReport, Dictionary<string, TableDefinition> tableDefinitions, Dictionary<string, string> filingIndicators, ModuleDefinition moduleDefinition)
        {
            var report = new Report
            {
                Entrypoint = Path.ChangeExtension(xmlReport.SchemaReference.Value, ".json")
            };

            var prefix = moduleDefinition.DocumentInfo.namespaces.FirstOrDefault(ns => ns.Value == xmlReport.Entity.Identifier.Scheme).Key;
            var identifier = xmlReport.Entity.Identifier.Value;
            report.Parameters.Add("entityID", $"{prefix}:{identifier}");
            report.Parameters.Add("refPeriod", xmlReport.Period.Instant.ToString("yyyy-MM-dd"));
            report.Parameters.Add("baseCurrency", xmlReport.Units.FirstOrDefault(u => u.Measure.Namespace == "http://www.xbrl.org/2003/iso4217")?.Measure?.LocalName() ?? "EUR");
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
                Where(table => report.FilingIndicators.GetValueOrDefault(filingIndicators[table.Key], false)).
                ToDictionary(t => t.Key, t => t.Value);

            var tablesOpendimensions =
                tableDefinitions.
                    Where(t => reportedTables.ContainsKey(t.Key)).
                    ToDictionary(
                        t => t.Key,
                        t => t.Value.TableTemplates.First().Value.Columns.FactValue.Dimensions.Select(d => d.Key.Split(':').Last()).ToHashSet());

            foreach (var fact in xmlReport.Facts)
            {
                var value = fact.Value;

                // Typed members are all open
                var openDimensions = fact.Context.Scenario.TypedMembers.ToDictionary(m => m.Dimension.LocalName(), m => m.Value);

                var factExplicitMembers = fact.Context.Scenario.ExplicitMembers.ToDictionary(m => m.Dimension.Name, m => m.Value.Name);
                var factTypedMembers = fact.Context.Scenario.TypedMembers.Select(m => m.Dimension.LocalName()).ToHashSet();

                var datapoints = GetTableDatapoints(fact, reportedTables, dimNsPrefix, tablesOpendimensions, factExplicitMembers, factTypedMembers);
                foreach (var table in datapoints)
                    foreach (var datapoint in table.Value)
                    {
                        foreach (var dim in tablesOpendimensions[table.Key])
                            if (!openDimensions.ContainsKey(dim))
                                // Some explicit members might be open, depending on the table
                                openDimensions[dim] = fact.Context.Scenario.ExplicitMembers.First(m => m.Dimension.Name == dim).MemberCode;
                        // factExplicitMembers[dim];

                        report.AddData(table.Key, datapoint, fact.Value, openDimensions);

                    }

            }

            return report;
        }

        private static Dictionary<string, string[]> GetTableDatapoints(
            Fact fact,
            Dictionary<string, TableDefinition> tableDefinitions,
            string dimNsPrefix,
            Dictionary<string, HashSet<string>> tablesOpenDimensions,
            Dictionary<string, string> factExplicitMembers,
            HashSet<string> factTypedMembers)
        {
            var metric = fact.Metric.Name;
            var result = new Dictionary<string, string[]>();

            foreach (var td in tableDefinitions)
            {
                var candidateDatapoints = td.Value.GetDatapointsByMetric(metric);

                if (candidateDatapoints.Any())
                {
                    var tableOpenDimensions = tablesOpenDimensions[td.Key];
                    // filter by matching explicit members
                    var matchingDatapoints =
                        candidateDatapoints.
                            Where(pg =>
                                DatapointMatchesFact(
                                    pg.Value.DimensionValues,
                                    tableOpenDimensions,
                                    factExplicitMembers,
                                    factTypedMembers)).
                            ToArray();

                    if (matchingDatapoints.Any())
                        result[td.Key] = matchingDatapoints.Select(dp => dp.Key).ToArray();
                }
            }

            return result;
        }

        private static bool DatapointMatchesFact(Dictionary<string, string> datapointDimensions, HashSet<string> tableOpenDimensions, Dictionary<string, string> factExplicitMembers, HashSet<string> factTypedMembers)
        {
            var match =
                factExplicitMembers.Count + factTypedMembers.Count == tableOpenDimensions.Count + datapointDimensions.Count
                &&
                factExplicitMembers.
                All(m => datapointDimensions.GetValueOrDefault(m.Key, "") == m.Value
                    || tableOpenDimensions.Contains(m.Key))
                &&
                factTypedMembers.All(m => tableOpenDimensions.Contains(m));

            return match;
        }
    }
}