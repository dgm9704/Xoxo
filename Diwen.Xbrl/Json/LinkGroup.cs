using System.Text.Json.Serialization;

namespace Diwen.Xbrl.Json
{
    public class LinkGroup
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string[] Value { get; set; }

    }
}