namespace Diwen.Xbrl.Json
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Fact
    {

        [JsonRequired]
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("decimals")]
        public int? Decimals { get; set; }

        [JsonRequired]
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("links")]
        public Dictionary<string, Dictionary<string, string[]>> Links { get; set; }
    }
}