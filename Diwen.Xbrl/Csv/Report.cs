namespace Diwen.Xbrl.Csv
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;

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

			report.Entrypoint = ReadEntryPoint(reportFiles[$"{packagename}/reports/report.json"]);
			report.Parameters = ReadParameters(reportFiles[$"{packagename}/reports/parameters.csv"]);
			report.FilingIndicators = ReadFilingIndicators(reportFiles[$"{packagename}/reports/FilingIndicators.csv"]);
			foreach (var table in report.FilingIndicators.Where(fi => fi.Value).Select(fi => fi.Key.ToLowerInvariant()))
				report.Data.AddRange(ReadTableData(table, reportFiles[$"{packagename}/reports/{table}.csv"]));

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
	}
}