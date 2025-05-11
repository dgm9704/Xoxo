namespace Diwen.Xbrl.Json
{
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Features
    {
        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("xbrl:canonicalValues")]
        public bool? CanonicalValues { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("xbrl:allowedDuplicates")]
        public string AllowedDuplicates { get; set; }

    }
}