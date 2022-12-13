namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;

    public class JsonModule
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

    public class INC
    {
    }

    public class EbaDocumentation
    {
        public string CellCode { get; set; }
        public string logicalDataPointId { get; set; }
    }

    public class PropertyGroup
    {
        public string decimals { get; set; }
        public Dictionary<string, string> dimensions { get; set; }

    }

    public class Datapoint
    {
        public Dictionary<string, PropertyGroup> propertyGroups { get; set; }
    }

    public class FactValue
    {
        public Dictionary<string, string> dimensions { get; set; }
        public IList<string> propertiesFrom { get; set; }
    }

    public class Columns
    {
        public INC INC { get; set; }
        public Datapoint datapoint { get; set; }
        public FactValue factValue { get; set; }
    }

    public class TableTemplate
    {
        public Columns columns { get; set; }
    }

    public class JsonTable
    {
        public DocumentInfo documentInfo { get; set; }
        public Dictionary<string, TableTemplate> tableTemplates { get; set; }
    }
}