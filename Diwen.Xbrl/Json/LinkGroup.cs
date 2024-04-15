using System.Text.Json.Serialization;

namespace Diwen.Xbrl.Json
{
    public class LinkGroup
    {

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string[] Value { get; set; } = [];

    }
}