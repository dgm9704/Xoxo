namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class FactValue
    {

        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        [JsonPropertyName("propertiesFrom")]
        public IList<string> PropertiesFrom { get; set; }
        
    }
}