namespace Diwen.Xbrl.Json
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class Report
    {
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [JsonPropertyName("facts")]
        public Dictionary<string, Fact> Facts { get; set; } = [];

        public static Report FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<Report>(stream);
        }

        public void ToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize<Report>(stream, this);
        }
    }
}


