namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Text.Json.Serialization;
    
    public class TableTemplate
    {

        [JsonPropertyName("columns")]
        public Columns Columns { get; set; }
    }
}