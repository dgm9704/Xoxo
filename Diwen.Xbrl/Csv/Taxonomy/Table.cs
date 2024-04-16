namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Text.Json.Serialization;

    public class Table
    {

        [JsonPropertyName("optional")]
        public bool Optional { get; set; }

        [JsonPropertyName("template")]
        public string Template { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

    }
}