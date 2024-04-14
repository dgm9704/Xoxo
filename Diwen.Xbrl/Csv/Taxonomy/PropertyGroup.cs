namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    public class PropertyGroup
    {

        [JsonPropertyName("decimals")]
        public string Decimals { get; set; }

        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        private readonly HashSet<string> excludeDimensions = new(["concept", "unit"]);

        private Dictionary<string, string> dimensionValues;

        public Dictionary<string, string> DimensionValues
        {
            get
            {
                if (dimensionValues == null)
                    dimensionValues =
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