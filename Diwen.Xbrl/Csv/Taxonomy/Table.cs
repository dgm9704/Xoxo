namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Table
    {
        /// <summary/>
        [JsonPropertyName("eba:documentation")]
        public Dictionary<string, object> EbaDocumentation { get; set; } = [];

        /// <summary/>
        [JsonPropertyName("optional")]
        public bool Optional { get; set; }

        /// <summary/>
        [JsonPropertyName("template")]
        public string Template { get; set; }

        /// <summary/>
        [JsonPropertyName("url")]
        public string Url { get; set; }

    }
}