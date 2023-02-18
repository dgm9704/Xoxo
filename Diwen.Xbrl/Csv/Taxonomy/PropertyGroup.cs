namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;

    public class PropertyGroup
    {
        public string decimals { get; set; }
        public Dictionary<string, string> dimensions { get; set; }

        private Dictionary<string, string> dimensionsField;
        private readonly HashSet<string> excludeDimensions = new HashSet<string>(new string[] { "concept", "unit" });
        public Dictionary<string, string> Dimensions
        {
            get
            {
                if (dimensionsField == null)
                    dimensionsField = 
                        dimensions.
                        Where(d => !excludeDimensions.Contains(d.Key)).
                        ToDictionary(
                            d => d.Key.Split(':').Last(), 
                            d => d.Value.Split(':').Last());

                return dimensionsField;
            }
        }
    }
}