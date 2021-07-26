namespace Diwen.XbrlCsv
{
	using System.Collections.Generic;
	using System.IO;
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

		public Report()
		=> DocumentInfo = new DocumentInfo();

		public void Export()
		{
			ExportDocumentInfo();

			ExportParameters();

			ExportFilingIndicators();
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

	}
}
