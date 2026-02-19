//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using Microsoft.VisualBasic.FileIO;
    using System.Reflection;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Csv.Taxonomy;
    using Diwen.Xbrl.Extensions;
    using Diwen.Xbrl.Xml;
    using Diwen.Xbrl.Package;

    /// <summary/>
    public class PlainCsvReport
    {
        /// <summary/>
        public string DocumentType { get; set; } = "https://xbrl.org/CR/2021-02-03/xbrl-csv";

        /// <summary/>
        public string Entrypoint { get; set; }

        /// <summary/>
        public Dictionary<string, bool> FilingIndicators = [];

        /// <summary/>
        public Dictionary<string, string> Parameters = [];

        /// <summary/>
        public List<ReportData> Data = [];

        private static readonly string[] separator = ["\r", "\n", "\r\n"];

        /// <summary/>
        public void AddData(string table, string datapoint, string value)
            => Data.Add(new ReportData(table, datapoint, value));

        /// <summary/>
        public void AddData(string table, string datapoint, string value, params (string key, string value)[] pairs)
            => Data.Add(new ReportData(table, datapoint, value, pairs));

        /// <summary/>
        public void AddData(string table, string datapoint, string value, string dimensionKey, string dimensionValue)
            => Data.Add(new ReportData(table, datapoint, value, dimensionKey, dimensionValue));

        /// <summary/>
        public void AddData(string table, string datapoint, string value, Dictionary<string, string> dimensions)
            => Data.Add(new ReportData(table, datapoint, value, dimensions));

        /// <summary/>
        public void Export(
            string packagePath,
            ModuleDefinition moduleDefinition)
        {
            var packageName = Path.GetFileNameWithoutExtension(packagePath);
            var package = CreatePackage(
                packageName,
                DocumentType,
                Entrypoint,
                Parameters,
                FilingIndicators,
                moduleDefinition,
                Data);
            var zip = CreateZip(package);
            WriteStreamToFile(zip, Path.ChangeExtension(packagePath, "zip"));
        }

        private static Dictionary<string, Stream> CreatePackage(
            string packageName,
            string documentType,
            string entrypoint,
            Dictionary<string, string> parameters,
            Dictionary<string, bool> filingIndicators,
            ModuleDefinition moduleDefinition,
            List<ReportData> data)
        {
            var metafolder = "META-INF";
            var reportfolder = "reports";

            var package = new Dictionary<string, Stream>
            {
                [$"{packageName}/{metafolder}/reportPackage.json"] = CreatePackageInfo(),
                [$"{packageName}/{reportfolder}/report.json"] = CreateReportInfo(documentType, entrypoint),
                [$"{packageName}/{reportfolder}/parameters.csv"] = CreateParameters(parameters),
                [$"{packageName}/{reportfolder}/FilingIndicators.csv"] = CreateFilingIndicators(filingIndicators),
            };

            foreach (var tableStream in CreateReportData(data, moduleDefinition))
                package.Add($"{packageName}/{reportfolder}/{tableStream.Key}", tableStream.Value);

            return package;
        }

        private static void WriteStreamToFile(Stream stream, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                stream.CopyTo(fileStream);
        }

        private static MemoryStream CreateZip(Dictionary<string, Stream> package)
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

        private static MemoryStream CreatePackageInfo()
        {
            var info = new PackageInfo
            {
                DocumentInfo = new DocumentInfo
                {
                    DocumentType = "http://xbrl.org/PWD/2020-12-09/report-package"
                }
            };

            var stream = new MemoryStream();
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            JsonSerializer.Serialize<PackageInfo>(stream, info, options);
            stream.Position = 0;
            return stream;
        }

        private static MemoryStream CreateReportInfo(string documentType, string entrypoint)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var version = assembly.Version;
            var assemblyName = assembly.Name;
            var compileTime = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);

            var documentInfo = new DocumentInfo
            {
                DocumentType = documentType,
                Extends = new List<string> { entrypoint }
            };

            var softwareInfo = new EbaGeneratingSoftwareInformation
            {
                EbaSoftwareId = assemblyName,
                EbaSoftwareVersion = version.ToString(),
                EbaSoftwareCreationDate = $"{compileTime.Date:yyyy-MM-dd}",
                EbaSoftwareAdditionalInfo = "https://github.com/dgm9704/Xoxo"
            };

            var stream = new MemoryStream();
            var reportInfo = new ReportInfo
            {
                DocumentInfo = documentInfo,
                EbaGeneratingSoftwareInformation = softwareInfo
            };

            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            JsonSerializer.Serialize(stream, reportInfo, options);
            stream.Position = 0;
            return stream;
        }

        private static MemoryStream CreateParameters(Dictionary<string, string> parameters)
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

        private static MemoryStream CreateFilingIndicators(Dictionary<string, bool> filingIndicators)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            var builder = new StringBuilder();
            builder.AppendLine("templateID,reported");
            foreach (var fi in filingIndicators)
                builder.AppendLine($"{fi.Key},{fi.Value.ToString().ToLower()}");
            writer.Write(builder.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static Dictionary<string, Stream> CreateReportData(
            List<ReportData> data,
            ModuleDefinition moduleDefinition)
        {
            var reportdata = new Dictionary<string, Stream>();
            var filingInfo = moduleDefinition.FilingInfo();
            var tableDefinitions = moduleDefinition.TableDefinitions();

            var tabledata = data.GroupBy(d => d.Table);
            foreach (var table in tabledata)
            {
                var filename = filingInfo[table.Key].Url;

                HashSet<string> headers = [];
                var tableDefinition = tableDefinitions[table.Key];
                foreach (var dimension in table.First().Dimensions.Select(d => d.Key).Order())
                {
                    var keyColumn = tableDefinition.TableTemplates.First().Value.Dimensions[dimension];
                    headers.Add(keyColumn.TrimStart('$'));
                }

                foreach (var column in table.Select(d => d.Datapoint).Order())
                    headers.Add(column);

                var builder = new StringBuilder(headers.Join(","));

                builder.AppendLine();

                foreach (var keycombination in table.GroupBy(i =>
                             i.Dimensions.OrderBy(d => d.Key).Select(d => d.Value).Join(",")))
                {
                    if (!string.IsNullOrEmpty(keycombination.Key))
                        builder.Append(keycombination.Key + ",");

                    var columnvalues = keycombination.OrderBy(i => i.Datapoint).Select(i => i.Value);
                    builder.AppendLine(columnvalues.Join(","));
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

        /// <summary/>
        public static PlainCsvReport FromFile(
            string packagePath,
            ModuleDefinition moduleDefinition)
        {

            var filingInfo = moduleDefinition.FilingInfo();
            var tableDefinitions = moduleDefinition.TableDefinitions();
            var report = new PlainCsvReport();
            var reportFiles = ReadPackage(packagePath);
            var packagename = Path.GetFileNameWithoutExtension(packagePath);

            report.Entrypoint = ReadEntryPoint(reportFiles.Single(f => f.Key.EndsWith("report.json")).Value);
            report.Parameters = ReadParameters(reportFiles.Single(f => f.Key.EndsWith("parameters.csv")).Value);
            report.FilingIndicators =
                ReadFilingIndicators(reportFiles.Single(f => f.Key.EndsWith("FilingIndicators.csv")).Value);

            foreach (var filingIndicatorCode in report.FilingIndicators.Where(fi => fi.Value).Select(fi => fi.Key))
            {
                foreach (var filing in filingInfo.Values.Where(f => f.Indicator == filingIndicatorCode))
                {
                    var url = filing.Url;
                    var templateCode = filing.Template;
                    var tablefile = reportFiles.SingleOrDefault(f => Path.GetFileName(f.Key) == url);
                    if (tablefile.Key != default)
                    {
                        var tableDefinition = tableDefinitions[templateCode];
                        var tabledata = ReadTableData(templateCode, tablefile.Value, tableDefinition);
                        report.Data.AddRange(tabledata);
                    }
                }
            }

            return report;
        }

        /// <summary/>
        public static string GetPackageEntryPoint(string packagePath)
        {
            using (var packageStream = File.OpenRead(packagePath))
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read))
            {
                var entry = archive.Entries.First(e => e.Name.Equals("report.json"));

                using (var entryStream = entry.Open())
                using (var memoryStream = new MemoryStream())
                {
                    entryStream.CopyTo(memoryStream);
                    string content = Encoding.UTF8.GetString(memoryStream.ToArray());
                    var reportInfo = JsonSerializer.Deserialize<ReportInfo>(content);
                    return reportInfo.DocumentInfo.Extends.First();
                }
            }
        }

        private static string ReadEntryPoint(string data)
        {
            var reportInfo = JsonSerializer.Deserialize<ReportInfo>(data);
            return reportInfo.DocumentInfo.Extends.First();
        }

        private static List<ReportData> ReadTableData(string table, string data, TableDefinition tableDefinition)
        {
            var result = new List<ReportData>();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            using (var parser = new TextFieldParser(stream, Encoding.UTF8, detectEncoding: true))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;

                var keyColumns = tableDefinition.TableTemplates.First().Value.Dimensions;
                var columns = parser.ReadFields();

                while (!parser.EndOfData)
                {
                    var row = parser.ReadFields();

                    Dictionary<string, string> dimensions = [];
                    foreach (var keycolumn in keyColumns)
                    {
                        var idx = Array.IndexOf(columns, keycolumn.Value.TrimStart('$'));
                        dimensions.Add(keycolumn.Key, row[idx]);
                    }

                    for (int i = 0; i < columns.Length; i++)
                    {
                        if (Array.IndexOf([.. keyColumns.Values], $"${columns[i]}") == -1)
                        {
                            var datapoint = columns[i];
                            var value = row[i];
                            var item = new ReportData(table, datapoint, value, dimensions);
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        private static Dictionary<string, bool> ReadFilingIndicators(string data)
            => data.Split(separator, StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(line => line.Split(','))
                .ToDictionary(
                    f => f[0],
                    f => Convert.ToBoolean(f[1]));

        private static Dictionary<string, string> ReadParameters(string data)
            => data.Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).Select(line => line.Split(',')).ToDictionary(
                    p => p[0],
                    p => p[1]);

        /// <summary/>
        public static Dictionary<string, string> ReadPackage(string packagePath)
        {
            var reportFiles = new Dictionary<string, string>();
            using (var packageStream = File.OpenRead(packagePath))
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    using (var entryStream = entry.Open())
                    using (var reader = new StreamReader(entryStream, Encoding.UTF8,
                               detectEncodingFromByteOrderMarks: true))
                    {
                        var content = reader.ReadToEnd();
                        reportFiles.Add(entry.FullName, content);
                    }
                }
            }

            return reportFiles;
        }

        private static string AddFactsForTable(
            Dictionary<string, string> parameters,
            TableDefinition tableDefinition,
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            HashSet<string> typedDomains,
            Xml.Report xmlreport,
            string baseCurrencyRef,
            KeyValuePair<string, ReportData[]> table,
            Dictionary<string, Context> usedContexts,
            HashSet<string> usedDatapoints)
        {
            string dimensionPrefix = string.Empty;

            foreach (var ns in tableDefinition.DocumentInfo.Namespaces)
            {
                if (ns.Key.EndsWith("_dim"))
                {
                    dimensionPrefix = ns.Key;
                    xmlreport.SetDimensionNamespace(ns.Key, ns.Value);
                }
                else if (ns.Key.EndsWith("_met"))
                {
                    xmlreport.SetMetricNamespace(ns.Key, ns.Value);
                }
                else
                {
                    xmlreport.AddDomainNamespace(ns.Key, ns.Value);
                }
            }

            foreach (var fact in table.Value)
            {
                var column = tableDefinition.Columns[fact.Datapoint];
                AddFact(parameters, dimensionDomain, typedDomainNamespace, typedDomains, xmlreport, baseCurrencyRef,
                    dimensionPrefix, column, fact, usedContexts, usedDatapoints);
            }

            return dimensionPrefix;
        }

        private static void AddFact(
            Dictionary<string, string> parameters,
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            HashSet<string> typedDomains,
            Xml.Report xmlreport,
            string baseCurrencyRef,
            string dimensionPrefix,
            Column column,
            ReportData fact,
            Dictionary<string, Context> usedContexts,
            HashSet<string> usedDatapoints)
        {
            var scenario = new Scenario(xmlreport);
            string metric = string.Empty;
            string unit = string.Empty;

            foreach (var dimension in column.Dimensions)
            {
                switch (dimension.Key)
                {
                    case "concept":
                        metric = dimension.Value;
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
            {
                var domain = dimensionDomain[d.Key.Split(':').Last()];
                if (typedDomains.Contains(domain))
                    scenario.AddTypedMember(d.Key, $"{typedDomainNamespace.Key}:{domain}", d.Value);
                else
                {
                    if (d.Value.IndexOf(':') == -1)
                        scenario.AddExplicitMember(d.Key, $"{domain}:{d.Value}");
                    else
                        scenario.AddExplicitMember(d.Key, d.Value);
                }
            }

            var decimals =
                !string.IsNullOrEmpty(column.Decimals)
                    ? parameters.GetValueOrDefault(column.Decimals.TrimStart('$'), string.Empty)
                    : string.Empty;

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
                    xmlreport.AddFact(context, metric, unitRef, decimals, fact.Value);
                    usedDatapoints.Add(datapointKey);
                }
            }
            else
            {
                context = xmlreport.CreateContext(scenario);
                usedContexts[scenarioKey] = context;
                xmlreport.AddFact(context, metric, unitRef, decimals, fact.Value);
                usedDatapoints.Add(datapointKey);
            }
        }

        private static Dictionary<string, string[]> GetTableDatapoints(
            Fact fact,
            Dictionary<string, TableDefinition> tableDefinitions,
            Dictionary<string, HashSet<string>> tablesOpenDimensions,
            Dictionary<string, string> factExplicitMembers,
            HashSet<string> factTypedMembers)
        {
            var metric = fact.Metric.Name;
            var result = new Dictionary<string, string[]>();

            foreach (var td in tableDefinitions)
            {
                var candidateColumns = td.Value.GetColumnsByMetric(metric);

                if (candidateColumns.Any())
                {
                    var tableOpenDimensions = tablesOpenDimensions[td.Key];
                    // filter by matching explicit members
                    var matchingDatapoints =
                        candidateColumns.Where(pg =>
                            DatapointMatchesFact(
                                pg.Value.DimensionValues,
                                tableOpenDimensions,
                                factExplicitMembers,
                                factTypedMembers)).ToArray();

                    if (matchingDatapoints.Any())
                        result[td.Key] = [.. matchingDatapoints.Select(dp => dp.Key)];
                }
            }

            return result;
        }

        private static bool DatapointMatchesFact(Dictionary<string, string> datapointDimensions,
            HashSet<string> tableOpenDimensions, Dictionary<string, string> factExplicitMembers,
            HashSet<string> factTypedMembers)
        {
            var match =
                factExplicitMembers.Count + factTypedMembers.Count ==
                tableOpenDimensions.Count + datapointDimensions.Count
                &&
                factExplicitMembers.All(m => datapointDimensions.GetValueOrDefault(m.Key, "") == m.Value
                                             || tableOpenDimensions.Contains(m.Key))
                &&
                factTypedMembers.All(m => tableOpenDimensions.Contains(m));

            return match;
        }

        /// <summary/>
        public static PlainCsvReport FromXbrlXml(
            Xml.Report xmlReport,
            ModuleDefinition moduleDefinition)
        {
            var report = new PlainCsvReport
            {
                Entrypoint = Path.ChangeExtension(xmlReport.SchemaReference.Value, ".json")
            };

            var prefix = moduleDefinition.DocumentInfo.Namespaces
                .FirstOrDefault(ns => ns.Value.ToString() == xmlReport.Entity.Identifier.Scheme).Key;
            var identifier = xmlReport.Entity.Identifier.Value;
            report.Parameters.Add("entityID", $"{prefix}:{identifier}");
            report.Parameters.Add("refPeriod", xmlReport.Period.Instant.ToString("yyyy-MM-dd"));
            report.Parameters.Add("baseCurrency",
                xmlReport.Units.FirstOrDefault(u => u.Measure.Namespace == "http://www.xbrl.org/2003/iso4217")
                    ?.Measure
                    ?.LocalName() ?? "EUR");
            report.Parameters.Add("decimalsInteger", "0");
            report.Parameters.Add("decimalsMonetary", "-3");
            report.Parameters.Add("decimalsPercentage", "4");
            report.Parameters.Add("decimalsDecimal", "2");

            foreach (var fi in xmlReport.FilingIndicators)
                report.FilingIndicators.Add(fi.Value, fi.Filed);

            var dimNsPrefix = xmlReport.Namespaces.LookupPrefix(
                xmlReport.Contexts.First(c => c.Scenario != null && c.Scenario.ExplicitMembers.Any()).Scenario
                    .ExplicitMembers.First().Dimension.Namespace);

            var tableDefinitions = moduleDefinition.TableDefinitions();

            var filingInfo = moduleDefinition.FilingInfo();

            var reportedTables =
                tableDefinitions
                    .Where(table => report.FilingIndicators.GetValueOrDefault(filingInfo[table.Key].Indicator, false))
                    .ToDictionary(t => t.Key, t => t.Value);

            var tablesOpendimensions =
                tableDefinitions.Where(t => reportedTables.ContainsKey(t.Key)).ToDictionary(
                    t => t.Key,
                    t => t.Value.TableTemplates.First().Value.Dimensions.Select(c => c.Key).ToHashSet());

            foreach (var fact in xmlReport.Facts.ToArray())
            {
                var value = fact.Value;
                // Typed members are all open

                var openDimensions =
                    fact.Context.Scenario.TypedMembers.ToDictionary(
                        m => $"{xmlReport.Namespaces.LookupPrefix(m.Dimension.Namespace)}:{m.Dimension.Name}",
                        m => m.Value);

                var factExplicitMembers =
                    fact.Context.Scenario.ExplicitMembers.ToDictionary(
                        m => $"{xmlReport.Namespaces.LookupPrefix(m.Dimension.Namespace)}:{m.Dimension.Name}",
                        m => $"{xmlReport.Namespaces.LookupPrefix(m.Value.Namespace)}:{m.Value.Name}");

                var factTypedMembers =
                    fact.Context.Scenario.TypedMembers.Select(m =>
                            $"{xmlReport.Namespaces.LookupPrefix(m.Dimension.Namespace)}:{m.Dimension.Name}")
                        .ToHashSet();

                var datapoints = GetTableDatapoints(fact, reportedTables, tablesOpendimensions, factExplicitMembers,
                    factTypedMembers);

                foreach (var table in datapoints)
                    foreach (var datapoint in table.Value)
                    {
                        foreach (var dim in tablesOpendimensions[table.Key])
                            if (!openDimensions.ContainsKey(dim))
                            {
                                var dimcode = dim.Split(':').Last();
                                // Some explicit members might be open, depending on the table
                                openDimensions[dim] = fact.Context.Scenario.ExplicitMembers
                                    .First(m => m.Dimension.Name == dimcode).MemberCode;
                            }

                        report.AddData(table.Key, datapoint, fact.Value, openDimensions);
                    }
            }

            return report;
        }

        /// <summary/>
        public Xml.Report ToXbrlXml(
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            HashSet<string> typedDomains,
            ModuleDefinition moduleDefinition)
            => ToXbrlXml(
                this,
                dimensionDomain,
                typedDomainNamespace,
                typedDomains,
                moduleDefinition);

        /// <summary/>
        public static Xml.Report ToXbrlXml(
            PlainCsvReport report,
            Dictionary<string, string> dimensionDomain,
            KeyValuePair<string, string> typedDomainNamespace,
            HashSet<string> typedDomains,
            ModuleDefinition moduleDefinition)
        {
            var tableDefinitions = moduleDefinition.TableDefinitions();
            var filingInfo = moduleDefinition.FilingInfo();
            var xmlreport = new Xml.Report
            {
                SchemaReference =
                    new SchemaReference("simple", moduleDefinition.DocumentInfo.Taxonomy.FirstOrDefault()?.ToString())
            };

            foreach (var ns in moduleDefinition.DocumentInfo.Namespaces)
                xmlreport.Namespaces.AddNamespace(ns.Key, ns.Value.ToString());

            var idParts = report.Parameters["entityID"].Split(':');
            var idNs = xmlreport.Namespaces.LookupNamespace(idParts.First());
            xmlreport.Entity = new Entity(idNs, idParts.Last());

            xmlreport.Period = new Period(DateTime.ParseExact(report.Parameters["refPeriod"], "yyyy-MM-dd",
                CultureInfo.InvariantCulture));

            var baseCurrency = report.Parameters["baseCurrency"];
            var baseCurrencyRef = $"u{baseCurrency.Split(':').Last()}";
            xmlreport.Units.Add(baseCurrencyRef, $"iso4217:{baseCurrency}");
            xmlreport.Units.Add("uPURE", "xbrli:pure");

            if (!string.IsNullOrEmpty(typedDomainNamespace.Key))
                xmlreport.SetTypedDomainNamespace(typedDomainNamespace.Key, typedDomainNamespace.Value);

            foreach (var fi in report.FilingIndicators)
                xmlreport.AddFilingIndicator(fi.Key, fi.Value);

            var filed =
                report.FilingIndicators.Where(i => i.Value).Select(i => i.Key).ToHashSet();

            var tabledata =
                report.Data.Where(d => !string.IsNullOrEmpty(d.Value))
                    .Where(d => filed.Contains(filingInfo[d.Table].Indicator)).GroupBy(d => d.Table)
                    .ToDictionary(d => d.Key, d => d.ToArray());

            var usedContexts = new Dictionary<string, Context>();
            var usedDatapoints = new HashSet<string>();

            foreach (var table in tabledata)
            {
                var tableDefinition = tableDefinitions[table.Key];
                AddFactsForTable(report.Parameters, tableDefinition, dimensionDomain, typedDomainNamespace,
                    typedDomains, xmlreport, baseCurrencyRef, table, usedContexts, usedDatapoints);
            }

            xmlreport.RemoveUnusedUnits();

            return xmlreport;
        }
    }
}