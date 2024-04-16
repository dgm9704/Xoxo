namespace Diwen.Xbrl.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Xml;
    using Diwen.Xbrl.Extensions;
    using Diwen.Xbrl.Xml;

    public class Report
    {

        [JsonRequired]
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("facts")]
        public Dictionary<string, Fact> Facts { get; set; }

        public static Report FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<Report>(stream);
        }

        public void ToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize<Report>(stream, this);
        }

        private static Dictionary<string, Uri> GetNamespaces(Xml.Report xmlreport)
                => xmlreport.Namespaces.
                    GetNamespacesInScope(XmlNamespaceScope.ExcludeXml).
                    ToDictionary(
                        ns => ns.Key,
                        ns => new Uri(ns.Value));

        private static Dictionary<string, string> GetDimensions(Xml.Fact fact, string dimensionPrefix)
        {
            var dimensions = new Dictionary<string, string>
            {
                ["concept"] = fact.Metric.Name,
                ["entity"] = $"lei:{fact.Context.Entity.Identifier.Value}",
                ["period"] = $"{fact.Context.Period.Instant:yyyy-MM-ddTHH:mm:ss}",

            };
            if (fact.Unit != null)
                dimensions["unit"] = $"iso4217:{fact.Unit.Measure.LocalName()}";

            foreach (var member in fact.Context.Scenario.ExplicitMembers)
                dimensions.Add($"{dimensionPrefix}:{member.Dimension.LocalName()}", $"{member.MemberCode}");

            foreach (var member in fact.Context.Scenario.TypedMembers)
                dimensions.Add($"{member.Dimension.Prefix()}:{member.Dimension.LocalName()}", member.Value);

            return dimensions;
        }

        public static Report FromXbrlXml(Xml.Report xmlreport)
        {
            var dimensionPrefix = xmlreport.Namespaces.LookupPrefix(xmlreport.DimensionNamespace);

            var report = new Report
            {
                DocumentInfo = new()
                {
                    DocumentType = "https://xbrl.org/2021/xbrl-json",
                    Namespaces = new(GetNamespaces(xmlreport))
                    {
                        ["lei"] = new Uri("http://standards.iso.org/iso/17442"),
                        ["iso4217"] = new Uri("http://www.xbrl.org/2003/iso4217")
                    },
                    Taxonomy =
                    [
                        new Uri(xmlreport.SchemaReference.Value)
                    ]
                },
                Facts = xmlreport.Facts.
                    Select((fact, index) => (fact, index)).
                    ToDictionary(
                    f => $"f{f.index + 1}",
                    f => new Fact()
                    {
                        Value = f.fact.Value,
                        Decimals = string.IsNullOrEmpty(f.fact.Decimals) ? null : Convert.ToInt32(f.fact.Decimals),
                        Dimensions = GetDimensions(f.fact, dimensionPrefix),
                    }
                    ),
            };

            return report;
        }
    }
}


