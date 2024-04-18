namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Datapoint
    {
        /// <summary/>
        [JsonPropertyName("propertyGroups")]
        public Dictionary<string, PropertyGroup> PropertyGroups { get; set; }
    }
}