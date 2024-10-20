namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Features
    {
        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("xbrl:allowedDuplicates")]
        public AllowedDuplicates? AllowedDuplicates { get; set; }
    }
}