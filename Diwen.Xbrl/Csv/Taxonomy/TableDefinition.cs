namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;

    public class TableDefinition
    {
        public DocumentInfo documentInfo { get; set; }
        public Dictionary<string, TableTemplate> tableTemplates { get; set; }

        public Dictionary<string, PropertyGroup> Datapoints => tableTemplates.First().Value.columns.datapoint.propertyGroups;

        
    }
}