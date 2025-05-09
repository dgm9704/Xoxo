namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Columns
    {
        private Dictionary<string, TableColumn> tablecolumns;

        /// <summary/>
        public Dictionary<string, TableColumn> TableColumns
        {
            get
            {
                tablecolumns ??= DynamicProperties.ToDictionary(
                        p => p.Key,
                        p => JsonSerializer.Deserialize<TableColumn>(p.Value.GetRawText()));

                return tablecolumns;
            }
        }

        /// <summary/>
        [JsonPropertyName("datapoint")]
        public Datapoint Datapoint { get; set; } = new();

        /// <summary/>
        [JsonPropertyName("factValue")]
        public FactValue FactValue { get; set; }

        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; } = [];

        [JsonExtensionData]
        public Dictionary<string, JsonElement> DynamicProperties { get; set; }
    }
}