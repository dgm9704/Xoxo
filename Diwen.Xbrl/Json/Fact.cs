namespace Diwen.Xbrl.Json
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Fact
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; } = [];

        [JsonPropertyName("links")]
        public Dictionary<string, LinkGroup> Links { get; set; } = [];
    }
}