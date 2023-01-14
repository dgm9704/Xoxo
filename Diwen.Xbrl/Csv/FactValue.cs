namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;

    public class FactValue
    {
        public Dictionary<string, string> dimensions { get; set; }
        public IList<string> propertiesFrom { get; set; }
    }
}