namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;
    using Diwen.Xbrl.Extensions;
    using Diwen.Xbrl.Package;

    public class TableDefinition
    {
        public DocumentInfo documentInfo { get; set; }
        public Dictionary<string, TableTemplate> tableTemplates { get; set; }

        private Dictionary<string, PropertyGroup> datapoints;
        public Dictionary<string, PropertyGroup> Datapoints
        {
            get
            {
                if (datapoints == null)
                    datapoints = tableTemplates.First().Value.columns.datapoint.propertyGroups;

                return datapoints;
            }
        }

        private Dictionary<string, KeyValuePair<string, PropertyGroup>[]> datapointsByMetric;
        private static KeyValuePair<string, PropertyGroup>[] noCandidates = new KeyValuePair<string, PropertyGroup>[] { };

        public KeyValuePair<string, PropertyGroup>[] GetDatapointsByMetric(string metric)
        {
            if (datapointsByMetric == null)
            {
                datapointsByMetric =
                    Datapoints.
                    GroupBy(pg => pg.Value.dimensions["concept"]).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToArray());
            }

            return datapointsByMetric.GetValueOrDefault(metric, noCandidates);
        }
    }
}