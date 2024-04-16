namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

    public class ReportInfo
    {
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [JsonPropertyName("eba:generatingSoftwareInformation")]
        public EbaGeneratingSoftwareInformation EbaGeneratingSoftwareInformation { get; set; }
    }
}