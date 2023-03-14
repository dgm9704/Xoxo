namespace Diwen.Xbrl.Package
{
    using System.Collections.Generic;

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
}