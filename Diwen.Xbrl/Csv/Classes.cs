namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;

    public class Module
    {
        public Dictionary<string, string> dimensions { get; set; }
        public DocumentInfo documentInfo { get; set; }
        public string parameterURL { get; set; }
        public Dictionary<string, string> parameters { get; set; }
        public Dictionary<string, Table> tables { get; set; }
    }

    public class DocumentInfo
    {
        public string documentType { get; set; }
        public List<string> extends { get; set; }
        public Dictionary<string, string> features { get; set; }
        public Dictionary<string, bool> final { get; set; }
        public Dictionary<string, string> linkGroups { get; set; }
        public Dictionary<string, string> linkTypes { get; set; }
        public Dictionary<string, string> namespaces { get; set; }
        public List<string> taxonomy { get; set; }
    }

    public class Table
    {
        public bool optional { get; set; }
        public string template { get; set; }
        public string url { get; set; }
    }
}