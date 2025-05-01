namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

    /// <summary/>
    public class ReportInfo
    {
        /// <summary/>
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        /// <summary/>
        [JsonPropertyName("eba:generatingSoftwareInformation")]
        public EbaGeneratingSoftwareInformation EbaGeneratingSoftwareInformation { get; set; }
    }
}