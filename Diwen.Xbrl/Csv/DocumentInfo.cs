namespace Diwen.Xbrl.Csv
{
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
        [JsonPropertyName("namespaces")]
        public Dictionary<string, string> Namespaces { get; set; }

        /// <summary/>
        [JsonPropertyName("linkTypes")]
        public Dictionary<string, string> LinkTypes { get; set; }

        /// <summary/>
        [JsonPropertyName("linkGroups")]
        public Dictionary<string, string> LinkGroups { get; set; }

        /// <summary/>
        [JsonPropertyName("taxonomy")]
        public List<string> Taxonomy { get; set; }

        /// <summary/>
        [JsonPropertyName("extends")]
        public List<string> Extends { get; set; }

        /// <summary/>
        [JsonPropertyName("final")]
        public Dictionary<string, bool> Final { get; set; }

        /// <summary/>
        [JsonPropertyName("features")]
        public Features Features { get; set; }

        /// <summary/>
        [JsonPropertyName("baseURL")]
        public string BaseUrl { get; set; }

    }
}