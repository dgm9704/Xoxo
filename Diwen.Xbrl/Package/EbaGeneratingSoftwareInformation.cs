namespace Diwen.Xbrl.Package
{
    using System.Text.Json.Serialization;
    
    public class EbaGeneratingSoftwareInformation
    {
        [JsonPropertyName("eba:softwareId")]
        public string ebasoftwareId { get; set; }

        [JsonPropertyName("eba:softwareVersion")]
        public string ebasoftwareVersion { get; set; }

        [JsonPropertyName("eba:softwareCreationDate")]
        public string ebasoftwareCreationDate { get; set; }

        [JsonPropertyName("eba:softwareAdditionalInfo")]
        public string ebasoftwareAdditionalInfo { get; set; }
    }
}