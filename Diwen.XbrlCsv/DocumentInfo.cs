namespace Diwen.XbrlCsv
{
	using System.Text.Json.Serialization;

	public class DocumentInfo
	{
		[JsonPropertyName("documentType")]
		public string DocumentType { get; set; } = "https://xbrl.org/CR/2021-02-03/xbrl-csv";

		[JsonPropertyName("extends")]
		public string Extends { get; set; }
	}
}