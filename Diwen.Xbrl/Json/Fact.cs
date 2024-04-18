namespace Diwen.Xbrl.Json
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Fact
    {

        /// <summary/>
        [JsonRequired]
        [JsonPropertyName("value")]
        public string Value { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("decimals")]
        public int? Decimals { get; set; }

        /// <summary/>
        [JsonRequired]
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("links")]
        public Dictionary<string, Dictionary<string, string[]>> Links { get; set; }
    }
}