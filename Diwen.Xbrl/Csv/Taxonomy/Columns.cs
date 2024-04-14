namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Text.Json.Serialization;
    
    public class Columns
    {
        [JsonPropertyName("datapoint")] 
        public Datapoint Datapoint { get; set; }

        [JsonPropertyName("factValue")] 
        public FactValue FactValue { get; set; }
    }
}