namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

        /// <summary/>
    public class FactValue
    {
        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        /// <summary/>
        [JsonPropertyName("propertiesFrom")]
        public IList<string> PropertiesFrom { get; set; }
        
    }
}