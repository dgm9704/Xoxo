namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

        /// <summary/>
    public class PackageInfo
    {
        /// <summary/>
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }
    }

}