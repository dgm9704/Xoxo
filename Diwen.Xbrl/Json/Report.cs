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

    /// <summary/>
    public class Report
    {
        private static readonly JsonSerializerOptions serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) },
            WriteIndented = true
        };

        /// <summary/>
        [JsonRequired]
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("facts")]
        public Dictionary<string, Fact> Facts { get; set; }

        /// <summary/>
        public static Report FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<Report>(stream, serializeOptions);
        }

        /// <summary/>
        public void ToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize<Report>(stream, this, serializeOptions);
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

        /// <summary/>
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

        /// <summary/>
        public Xml.Report ToXbrlXml(Dictionary<string, string> dimensionDomain, KeyValuePair<string, string> typedDomainNamespace, HashSet<string> typedDomains)
        => ToXbrlXml(this, dimensionDomain, typedDomainNamespace, typedDomains);

        /// <summary/>
        public static Xml.Report ToXbrlXml(Report report, Dictionary<string, string> dimensionDomain, KeyValuePair<string, string> typedDomainNamespace, HashSet<string> typedDomains)
        {
            var xmlreport = new Xml.Report
            {
                SchemaReference = new SchemaReference("simple", report.DocumentInfo.Taxonomy.Single().ToString())
            };

            string dimensionPrefix = string.Empty;

            foreach (var ns in report.DocumentInfo.Namespaces)
            {
                if (ns.Key.EndsWith("_dim"))
                {
                    dimensionPrefix = ns.Key;
                    xmlreport.SetDimensionNamespace(ns.Key, ns.Value);
                }
                else if (ns.Key.EndsWith("_met"))
                {
                    xmlreport.SetMetricNamespace(ns.Key, ns.Value);
                }
                else
                {
                    xmlreport.AddDomainNamespace(ns.Key, ns.Value);
                }
            }

            xmlreport.SetTypedDomainNamespace(typedDomainNamespace.Key, typedDomainNamespace.Value);

            foreach (var fact in report.Facts.Values)
            {
                var entityParts = fact.Dimensions["entity"].Split(':');
                var entityNamespace = report.DocumentInfo.Namespaces[entityParts.First()].ToString();
                var entity = new Entity(entityNamespace, entityParts.Last());
                var scenario = new Scenario(xmlreport);
                foreach (var dimension in fact.Dimensions.Where(d => d.Key.Contains(':')))
                {
                    //"Taxonomy dimensions"
                    var dimensionCode = dimension.Key.Split(':').Last();
                    if (typedDomains.Contains(dimensionDomain[dimensionCode]))
                        scenario.AddTypedMember(dimension.Key, $"{typedDomainNamespace.Key}:{dimensionDomain[dimensionCode]}", dimension.Value);
                    else
                        scenario.AddExplicitMember(dimension.Key, dimension.Value);
                }

                var period = new Period(DateTime.Parse(fact.Dimensions["period"]));

                var context = xmlreport.CreateContext(scenario);
                context.Entity = entity;
                context.Period = period;

                var metric = fact.Dimensions["concept"].Split(':').Last();
                var unitValue = fact.Dimensions.GetValueOrDefault("unit");
                string unitRef = null;
                if (unitValue != null)
                {
                    unitRef = $"u{unitValue.Split(':').Last().ToUpperInvariant()}";
                    var unit = new Unit(unitRef, unitValue);
                    xmlreport.Units.Add(unit);
                }
                var decimals = fact.Decimals?.ToString();
                var value = fact.Value;

                xmlreport.AddFact(context, metric, unitRef, decimals, value);
            }

            return xmlreport;
        }
    }
}


