namespace Diwen.XbrlCsv
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	public class Report
	{
		[JsonPropertyName("documentInfo")]
		public DocumentInfo DocumentInfo { get; set; }// = new DocumentInfo();

		[JsonIgnore]
		public Dictionary<string, bool> FilingIndicators = new Dictionary<string, bool>();

		[JsonIgnore]
		public Dictionary<string, string> Parameters = new Dictionary<string, string>();

		[JsonIgnore]
		public List<ReportData> Data = new List<ReportData>();

		public Report()
		=> DocumentInfo = new DocumentInfo();

		public void AddData(string table, string datapoint, string value)
		=> Data.Add(new ReportData(table, datapoint, value));

		public void AddData(string table, string datapoint, string value, params (string key, string value)[] pairs)
		=> Data.Add(new ReportData(table, datapoint, value, pairs));

		public void AddData(string table, string datapoint, string value, string dimensionKey, string dimensionValue)
		=> Data.Add(new ReportData(table, datapoint, value));

		public void AddData(string table, string datapoint, string value, Dictionary<string, string> dimensions)
		=> Data.Add(new ReportData(table, datapoint, value, dimensions));

		public void Export()
		{
			ExportDocumentInfo();

			ExportParameters();

			ExportFilingIndicators();

			ExportReportData();
		}

		private void ExportDocumentInfo()
		{
			string fileName = "report.json";
			var options = new JsonSerializerOptions { WriteIndented = true };
			string jsonString = JsonSerializer.Serialize<Report>(this, options);
			File.WriteAllText(fileName, jsonString);
		}

		private void ExportParameters()
		{
			var fileName = "parameters.csv";
			var data = new StringBuilder();
			data.AppendLine("name,value");
			foreach (var p in Parameters)
				data.AppendLine($"{p.Key},{p.Value}");
			File.WriteAllText(fileName, data.ToString());
		}

		private void ExportFilingIndicators()
		{
			var fileName = "FilingIndicators.csv";
			var data = new StringBuilder();
			data.AppendLine("templateId,reported");
			foreach (var fi in FilingIndicators)
				data.AppendLine($"{fi.Key},{fi.Value.ToString().ToLower()}");
			File.WriteAllText(fileName, data.ToString());
		}

		private void ExportReportData()
		{
			foreach (var template in FilingIndicators.Where(fi => fi.Value))
			{
				var tabledata = Data.Where(d => d.Table == template.Key);
				if (tabledata.Any())
				{
					var fileName = Path.ChangeExtension(template.Key, "csv");
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
					File.WriteAllText(fileName, output.ToString());
				}
			}
		}
	}
}
