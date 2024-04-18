namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Columns
    {
        /// <summary/>
        [JsonPropertyName("datapoint")]
        public Datapoint Datapoint { get; set; }

        /// <summary/>
        [JsonPropertyName("factValue")]
        public FactValue FactValue { get; set; }
    }
}