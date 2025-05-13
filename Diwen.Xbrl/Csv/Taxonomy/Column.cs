namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Column
    {

        /// <summary/>
        [JsonPropertyName("decimals")]
        public string Decimals { get; set; }

        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; } = [];

        private readonly HashSet<string> excludeDimensions = new(["concept", "unit"]);

        private Dictionary<string, string> dimensionValues;

        /// <summary/>
        public Dictionary<string, string> DimensionValues
        {
            get
            {
                dimensionValues ??=
                        Dimensions.
                        Where(d => !excludeDimensions.Contains(d.Key)).
                        ToDictionary(
                            d => d.Key,
                            d => d.Value);

                return dimensionValues;
            }
        }
    }
}