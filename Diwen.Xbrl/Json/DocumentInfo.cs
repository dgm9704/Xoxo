namespace Diwen.Xbrl.Json
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class DocumentInfo
    {
        [JsonRequired]
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("features")]
        public Dictionary<string, string> Features { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("namespaces")]
        public Dictionary<string, Uri> Namespaces { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("linkTypes")]
        public Dictionary<string, Uri> LinkTypes { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("linkGroups")]
        public Dictionary<string, Uri> LinkGroups { get; set; }

        [JsonRequired]
        [JsonPropertyName("taxonomy")]
        public Uri[] Taxonomy { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("baseURL")]
        public string BaseUrl { get; set; }


    }
}