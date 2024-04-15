namespace Diwen.Xbrl.Json
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Text.Json.Serialization;

    public class DocumentInfo
    {
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; } = string.Empty;

        [JsonPropertyName("features")]
        public Dictionary<string, string> Features { get; set; } = [];

        [JsonPropertyName("namespaces")]
        public Dictionary<string, Uri> Namespaces { get; set; } = [];

        [JsonPropertyName("linkTypes")]
        public Dictionary<string, Uri> LinkTypes { get; set; } = [];

        [JsonPropertyName("linkGroups")]
        public Dictionary<string, Uri> LinkGroups { get; set; } = [];

        [JsonPropertyName("taxonomy")]
        public Uri[] Taxonomy { get; set; } = [];

        [JsonPropertyName("baseURL")]
        public string BaseUrl { get; set; } = string.Empty;


    }
}