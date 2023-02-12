namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;

    public class TableDefinition
    {
        public DocumentInfo documentInfo { get; set; }
        public Dictionary<string, TableTemplate> tableTemplates { get; set; }
    }
}