namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Package;

    public class ModuleDefinition
    {

        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [JsonPropertyName("parameterURL")]
        public string ParameterURL { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        [JsonPropertyName("tables")]
        public Dictionary<string, Table> Tables { get; set; }
        
    }
}