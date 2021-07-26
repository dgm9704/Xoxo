namespace Diwen.XbrlCsv
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	public class Report
	{
		[JsonPropertyName("documentInfo")]
		public DocumentInfo DocumentInfo = new DocumentInfo();

		[JsonIgnore]
		public Dictionary<string, bool> FilingIndicators = new Dictionary<string, bool>();

		public void Export()
		{
			string fileName = "report.json";
			var options = new JsonSerializerOptions { WriteIndented = true };
			string jsonString = JsonSerializer.Serialize<Report>(this, options);
			File.WriteAllText(fileName, jsonString);

			fileName = "FilingIndicators.csv";
			var data = new StringBuilder();
			data.AppendLine("templateId,reported");
			foreach (var fi in FilingIndicators)
				data.AppendLine($"{fi.Key},{fi.Value.ToString().ToLower()}");
			File.WriteAllText(fileName, data.ToString());
		}
	}
}
