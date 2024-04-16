namespace Diwen.Xbrl.Csv
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class DocumentInfo
    {
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        [JsonPropertyName("extends")]
        public List<string> Extends { get; set; }

        [JsonPropertyName("features")]
        public Dictionary<string, string> Features { get; set; }

        [JsonPropertyName("final")]
        public Dictionary<string, bool> Final { get; set; }

        [JsonPropertyName("linkGroups")]
        public Dictionary<string, string> LinkGroups { get; set; }

        [JsonPropertyName("linkTypes")]
        public Dictionary<string, string> LinkTypes { get; set; }

        [JsonPropertyName("namespaces")]
        public Dictionary<string, string> Namespaces { get; set; }

        [JsonPropertyName("taxonomy")]
        public List<string> Taxonomy { get; set; }
    }
}