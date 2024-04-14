namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Datapoint
    {
        [JsonPropertyName("propertyGroups")]
        public Dictionary<string, PropertyGroup> PropertyGroups { get; set; }
    }
}