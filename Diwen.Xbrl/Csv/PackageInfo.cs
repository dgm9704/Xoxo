namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

    public class PackageInfo
    {

        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }
    }

}