namespace Diwen.Xbrl.Json
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class DocumentInfo
    {
        /// <summary/>
        [JsonRequired]
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("features")]
        public Dictionary<string, string> Features { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("namespaces")]
        public Dictionary<string, Uri> Namespaces { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("linkTypes")]
        public Dictionary<string, Uri> LinkTypes { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("linkGroups")]
        public Dictionary<string, Uri> LinkGroups { get; set; }

        /// <summary/>
        [JsonRequired]
        [JsonPropertyName("taxonomy")]
        public Uri[] Taxonomy { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("baseURL")]
        public string BaseUrl { get; set; }


    }
}