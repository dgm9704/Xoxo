namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;

    public class ModuleDefinition
    {
        public Dictionary<string, string> dimensions { get; set; }
        public DocumentInfo documentInfo { get; set; }
        public string parameterURL { get; set; }
        public Dictionary<string, string> parameters { get; set; }
        public Dictionary<string, Table> tables { get; set; }
    }
}