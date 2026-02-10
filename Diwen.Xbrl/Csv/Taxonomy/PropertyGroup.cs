namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Json;

    /// <summary/>
    public class PropertyGroup : EsaDocumented
    {
        /// <summary/>
        [JsonPropertyName("decimals")]
        public string Decimals { get; set; }

        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        private readonly HashSet<string> excludeDimensions = ["concept", "unit"];

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
                        d => d.Key.Split(':').Last(),
                        d => d.Value.Split(':').Last());

                return dimensionValues;
            }
        }
    }
}