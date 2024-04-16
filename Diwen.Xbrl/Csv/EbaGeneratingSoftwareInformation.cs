namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

    public class EbaGeneratingSoftwareInformation
    {
        [JsonPropertyName("eba:softwareId")]
        public string EbaSoftwareId { get; set; }

        [JsonPropertyName("eba:softwareVersion")]
        public string EbaSoftwareVersion { get; set; }

        [JsonPropertyName("eba:softwareCreationDate")]
        public string EbaSoftwareCreationDate { get; set; }

        [JsonPropertyName("eba:softwareAdditionalInfo")]
        public string EbaSoftwareAdditionalInfo { get; set; }
    }
}