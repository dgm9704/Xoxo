namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary/>    
    public class TableTemplate
    {
        /// <summary/>
        [JsonPropertyName("columns")]
        public Columns Columns { get; set; }

        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; } = [];

    }
}