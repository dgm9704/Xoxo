namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

        /// <summary/>
    public class EbaGeneratingSoftwareInformation
    {
                /// <summary/>
        [JsonPropertyName("eba:softwareId")]
        public string EbaSoftwareId { get; set; }

        /// <summary/>
        [JsonPropertyName("eba:softwareVersion")]
        public string EbaSoftwareVersion { get; set; }

        /// <summary/>
        [JsonPropertyName("eba:softwareCreationDate")]
        public string EbaSoftwareCreationDate { get; set; }

        /// <summary/>
        [JsonPropertyName("eba:softwareAdditionalInfo")]
        public string EbaSoftwareAdditionalInfo { get; set; }
    }
}