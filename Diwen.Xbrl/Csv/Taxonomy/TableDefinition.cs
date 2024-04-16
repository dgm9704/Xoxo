namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Extensions;

    public class TableDefinition
    {

        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [JsonPropertyName("tableTemplates")]
        public Dictionary<string, TableTemplate> TableTemplates { get; set; }

        private Dictionary<string, PropertyGroup> datapoints;
        public Dictionary<string, PropertyGroup> Datapoints
        {
            get
            {
                if (datapoints == null)
                    datapoints = TableTemplates.First().Value.Columns.Datapoint.PropertyGroups;

                return datapoints;
            }
        }

        private Dictionary<string, KeyValuePair<string, PropertyGroup>[]> datapointsByMetric;
        private static readonly KeyValuePair<string, PropertyGroup>[] noCandidates = [];

        public KeyValuePair<string, PropertyGroup>[] GetDatapointsByMetric(string metric)
        {
            if (datapointsByMetric == null)
            {
                datapointsByMetric =
                    Datapoints.
                    GroupBy(pg => pg.Value.Dimensions["concept"]).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToArray());
            }

            return datapointsByMetric.GetValueOrDefault(metric, noCandidates);
        }

        public static TableDefinition FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<TableDefinition>(stream);
        }
    }
}