using System.Text.Json.Serialization;

namespace Diwen.Xbrl.Csv.Taxonomy
{
    public class Columns
    {
        [JsonPropertyName("datapoint")] 
        public Datapoint Datapoint { get; set; }

        [JsonPropertyName("factValue")] 
        public FactValue FactValue { get; set; }
    }
}