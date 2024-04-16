using System.Text.Json.Serialization;

namespace Diwen.Xbrl.Package
{
    public class PackageInfo
    {

        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }
    }

}