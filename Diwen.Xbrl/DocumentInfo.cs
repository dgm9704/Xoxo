namespace Diwen.XbrlCsv
{
	// using System.Text.Json.Serialization;

	public class DocumentInfo
	{
		// [JsonPropertyName("documentType")]
		public string DocumentType { get; set; }

		// [JsonPropertyName("extends")]
		public string[] Extends { get; set; }
	}
}