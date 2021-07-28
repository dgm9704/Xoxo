namespace Diwen.XbrlCsv
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;

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
			var metafolder = Path.Combine(packagename, "META-INF");
			var reportfolder = Path.Combine(packagename, "reports");
			Directory.CreateDirectory(metafolder);
			Directory.CreateDirectory(reportfolder);

			ExportPackageInfo(metafolder);

			ExportReportInfo(reportfolder);

			ExportParameters(reportfolder);

			ExportFilingIndicators(reportfolder);

			ExportReportData(reportfolder);
		}

		private void ExportPackageInfo(string folderpath)
		{
			var filename = "reports.json";
			var filepath = Path.Combine(folderpath, filename);
			var content = "{\"documentInfo\":{\"documentType\":\"http://xbrl.org/PWD/2020-12-09/report-package\"}}";
			File.WriteAllText(filepath, content);
		}

		private void ExportReportInfo(string folderpath)
		{
			var filename = "report.json";
			var filepath = Path.Combine(folderpath, filename);
			var content = $"{{\"documentInfo\":{{\"documentType\":\"{DocumentType}}}\",\"extends\":[\"{Entrypoint}\"]}}}}";
			File.WriteAllText(filepath, content);
		}

		private void ExportParameters(string folderpath)
		{
			var filename = "parameters.csv";
			var filepath = Path.Combine(folderpath, filename);
			var data = new StringBuilder();
			data.AppendLine("name,value");
			foreach (var p in Parameters)
				data.AppendLine($"{p.Key},{p.Value}");
			File.WriteAllText(filepath, data.ToString());
		}

		private void ExportFilingIndicators(string folderpath)
		{
			var filename = "FilingIndicators.csv";
			var filepath = Path.Combine(folderpath, filename);
			var data = new StringBuilder();
			data.AppendLine("templateId,reported");
			foreach (var fi in FilingIndicators)
				data.AppendLine($"{fi.Key},{fi.Value.ToString().ToLower()}");
			File.WriteAllText(filepath, data.ToString());
		}

		private void ExportReportData(string folderpath)
		{
			foreach (var template in FilingIndicators.Where(fi => fi.Value))
			{
				var tabledata = Data.Where(d => d.Table == template.Key);
				if (tabledata.Any())
				{
					var filename = Path.ChangeExtension(template.Key, "csv");
					var filepath = Path.Combine(folderpath, filename);
					var output = new StringBuilder("datapoint,factvalue");
					foreach (var dimension in tabledata.First().Dimensions.Keys)
						output.Append($",{dimension}");
					output.AppendLine();

					foreach (var item in tabledata)
					{
						output.AppendFormat($"{item.Datapoint},{item.Value}");
						foreach (var dimension in item.Dimensions.Values)
							output.Append($",{dimension}");
						output.AppendLine();
					}
					File.WriteAllText(filepath, output.ToString());
				}
			}
		}
	}
}
