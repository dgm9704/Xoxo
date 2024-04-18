namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Text.Json.Serialization;

        /// <summary/>    
    public class TableTemplate
    {
        /// <summary/>
        [JsonPropertyName("columns")]
        public Columns Columns { get; set; }
    }
}