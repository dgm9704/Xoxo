namespace Diwen.Xbrl.Json
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class DocumentInfo
    {
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        [JsonPropertyName("namespaces")]
        public Dictionary<string, string> Namespaces { get; set; }

        [JsonPropertyName("taxonomy")]
        public List<string> Taxonomy { get; set; }
    }
}