namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Table
    {

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