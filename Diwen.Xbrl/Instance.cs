//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2018 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Instance : IEquatable<Instance>
    {
        static AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        static Version version = assembly.Version;
        static string id = assembly.Name;

        internal static Dictionary<string, string> DefaultXbrlNamespaces = new Dictionary<string, string>
        {
            ["xsi"] = "http://www.w3.org/2001/XMLSchema-instance",
            ["xbrli"] = "http://www.xbrl.org/2003/instance",
            ["link"] = "http://www.xbrl.org/2003/linkbase",
            ["xlink"] = "http://www.w3.org/1999/xlink",
            ["xbrldi"] = "http://xbrl.org/2006/xbrldi"
        };

        internal static Dictionary<string, string> DefaultUnitNamespaces = new Dictionary<string, string>
        {
            ["iso4217"] = "http://www.xbrl.org/2003/iso4217",
        };

        internal static Dictionary<string, string> DefaultIndicatorNamespaces = new Dictionary<string, string>
        {
            ["find"] = "http://www.eurofiling.info/xbrl/ext/filing-indicators",
        };

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlSerializerNamespaces { get; set; }

        [XmlIgnore]
        public XmlNamespaceManager Namespaces { get; set; }

        Entity entityField = new Entity();

        [XmlIgnore]
        public Entity Entity
        {
            get => entityField;
            set
            {
                entityField = value;
                entityField.Instance = this;
            }
        }

        [XmlIgnore]
        public Period Period { get; set; } = new Period();

        [XmlIgnore]
        public string TaxonomyVersion { get; set; }

        [XmlIgnore]
        public string InstanceGenerator { get; private set; }

        [XmlIgnore]
        public string FactNamespace { get; private set; }

        [XmlIgnore]
        public string DimensionNamespace { get; private set; }

        [XmlIgnore]
        public string TypedDomainNamespace { get; private set; }

        [XmlElement("schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
        public SchemaReference SchemaReference { get; set; } = new SchemaReference();

        [XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
        public UnitCollection Units { get; set; }

        [XmlArray("fIndicators", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        [XmlArrayItem("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        public FilingIndicatorCollection FilingIndicators { get; set; } = new FilingIndicatorCollection();

        [XmlIgnore]
        public bool FilingIndicatorsSpecified
        => FilingIndicators != null && FilingIndicators.Any();

        [XmlElement("context", Namespace = "http://www.xbrl.org/2003/instance")]
        public ContextCollection Contexts { get; set; }

        [XmlIgnore]
        public FactCollection Facts { get; private set; }

        [XmlAnyElement]
        public XmlElement[] FactItems
        {
            get => Facts.Select(f => f.ToXmlElement()).ToArray();
            set => GetElementTree(Facts, value);
        }

        internal static void GetElementTree(ICollection<Fact> facts, IEnumerable<XmlElement> value)
        {
            if (value != null)
            {
                foreach (var element in value)
                {
                    var fact = Fact.FromXmlElement(element);
                    facts.Add(fact);

                    var children = element.ChildNodes.OfType<XmlElement>();

                    if (children.Any())
                        GetElementTree(fact.Facts, children);
                }
            }
        }

        [XmlIgnore]
        public Collection<string> Comments { get; private set; }

        [XmlIgnore]
        public bool CheckExplicitMemberDomainExists { get; set; }

        public void SetDimensionNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
                throw new ArgumentNullException(nameof(namespaceUri));

            SetDimensionNamespace(prefix, namespaceUri.ToString());
        }

        public void SetDimensionNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
            DimensionNamespace = namespaceUri;
        }

        public void SetMetricNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
                throw new ArgumentNullException(nameof(namespaceUri));

            SetMetricNamespace(prefix, namespaceUri.ToString());
        }

        public void SetMetricNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
            FactNamespace = namespaceUri;
        }

        public void SetTypedDomainNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
                throw new ArgumentNullException(nameof(namespaceUri));

            SetTypedDomainNamespace(prefix, namespaceUri.ToString());
        }

        public void SetTypedDomainNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
            TypedDomainNamespace = namespaceUri;
        }

        public void AddDomainNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
                throw new ArgumentNullException(nameof(namespaceUri));

            AddDomainNamespace(prefix, namespaceUri.ToString());
        }

        public void AddDomainNamespace(string prefix, string namespaceUri)
        => Namespaces.AddNamespace(prefix, namespaceUri);

        public Context GetContext(Scenario scenario)
        {
            Context context;

            context = scenario == null
                ? Contexts.FirstOrDefault(c => c.Scenario == null)
                : Contexts.FirstOrDefault(c => scenario.Equals(c.Scenario));

            if (context == null)
            {
                context = new Context(scenario);
                Contexts.Add(context);
            }

            return context;
        }

        public Context GetContext(Segment segment)
        {
            Context context;

            context = segment == null
                ? Contexts.FirstOrDefault(c => c.Entity.Segment == null)
                : Contexts.FirstOrDefault(c => segment.Equals(c.Entity.Segment));

            if (context == null)
            {
                context = new Context(Entity, segment);
                Contexts.Add(context);
            }

            return context;
        }

        void AddDefaultNamespaces()
        {
            foreach (var item in DefaultXbrlNamespaces)
                Namespaces.AddNamespace(item.Key, item.Value);

            foreach (var item in DefaultIndicatorNamespaces)
                Namespaces.AddNamespace(item.Key, item.Value);
        }

        public void RemoveUnusedUnits()
        {
            var usedIds = new HashSet<string>();
            GetUsedUnits(Facts, usedIds);
            Units.RemoveUnusedItems(usedIds);
        }

        internal static void GetUsedUnits(FactCollection facts, HashSet<string> usedIds)
        {
            foreach (var fact in facts.Where(f => f.Unit != null || f.UnitRef != "" || f.Facts.Any()))
                if (!fact.Facts.Any())
                    if (fact.Unit == null)
                        usedIds.Add(fact.UnitRef);
                    else
                        usedIds.Add(fact.Unit.Id);
                else
                    GetUsedUnits(fact.Facts, usedIds);
        }

        public void RemoveUnusedObjects()
        {
            RemoveUnusedUnits();
            RemoveUnusedContexts();
        }

        public void CollapseDuplicateContexts()
        {
            var duplicates = Contexts.
                GroupBy(c => c).
                Where(g => g.Skip(1).Any());

            if (duplicates.Any())
            {
                foreach (var group in duplicates)
                    foreach (var duplicate in group.Skip(1))
                        Facts.
                             Where(f => f.Context.Id == duplicate.Id).
                             ToList().
                             ForEach(f => f.Context = group.First());

                RemoveUnusedContexts();
            }
        }

        public void RemoveUnusedContexts()
        {
            var usedIds = new HashSet<string>();
            var contextIds = Facts.
                Where(f => f.Context != null).
                Select(f => f.Context.Id).
                Concat(FilingIndicators.Select(f => f.ContextRef)).
                Distinct().
                ToList();

            usedIds.UnionWith(contextIds);

            GetUsedContexts(Facts, usedIds);

            Contexts.RemoveUnusedItems(usedIds);
        }

        internal void GetUsedContexts(FactCollection facts, HashSet<string> usedIds)
        {
            foreach (var fact in facts.Where(f => f.Context != null || f.Facts.Any()))
                if (fact.Facts.Any())
                    GetUsedContexts(fact.Facts, usedIds);
                else
                    usedIds.Add(fact.Context.Id);
        }

        public Instance()
        {
            Namespaces = new XmlNamespaceManager(new NameTable());
            AddDefaultNamespaces();
            Units = new UnitCollection(this);
            Contexts = new ContextCollection(this);
            Facts = new FactCollection(this);
            Comments = new Collection<string>();
        }

        #region IEquatable implementation

        public bool Equals(Instance other)
        {
            var result = false;
            if (other != null)
                if (SchemaReference.Equals(other.SchemaReference))
                    if (Units.Equals(other.Units))
                        if (FilingIndicators.Equals(other.FilingIndicators))
                            if (Contexts.Equals(other.Contexts))
                                result |= Facts.Equals(other.Facts);

            return result;
        }

        public override int GetHashCode()
        => SchemaReference.GetHashCode()
            ^ Units.GetHashCode()
            ^ FilingIndicators.GetHashCode();

        #endregion

        public override bool Equals(object obj)
        => Equals(obj as Instance);

        void RebuildNamespacesAfterRead()
        {
            var namespaces = new XmlNamespaceManager(new NameTable());

            foreach (var item in XmlSerializerNamespaces.ToArray())
                namespaces.AddNamespace(item.Name, item.Namespace);

            var contextsWithMembers = Contexts.Where(c => c.Scenario != null);
            DimensionFromTypedMembers(namespaces, contextsWithMembers);
            DimensionFromExplicitMembers(namespaces, contextsWithMembers);

            GetFactNamespace();

            UpdateContextNamespaces(namespaces, contextsWithMembers);

            Namespaces = namespaces;

            // Reset so any extras in the input don't make it out again
            XmlSerializerNamespaces = new XmlSerializerNamespaces();
        }

        void SetContextReferences(FactCollection facts)
        {
            foreach (var filingIndicator in FilingIndicators.Where(i => i.Context == null))
            {
                var contextRef = filingIndicator.ContextRef;
                if (!string.IsNullOrEmpty(contextRef))
                {
                    if (!Contexts.Contains(contextRef))
                        throw new KeyNotFoundException($"Referenced context '{contextRef}' does not exist");

                    filingIndicator.Context = Contexts[contextRef];
                }
            }

            foreach (var fact in facts)
            {
                if (fact.Context == null)
                {
                    var contextRef = fact.ContextRef;
                    if (!string.IsNullOrEmpty(contextRef))
                    {
                        if (!Contexts.Contains(contextRef))
                            throw new KeyNotFoundException($"Referenced context '{contextRef}' does not exist");

                        fact.Context = Contexts[contextRef];
                    }
                }

                if (fact.Facts.Any())
                    SetContextReferences(fact.Facts);
            }
        }

        void SetUnitReferences(FactCollection facts)
        {
            foreach (var fact in facts)
            {
                if (fact.Unit == null)
                {
                    var unitRef = fact.UnitRef;
                    if (!string.IsNullOrEmpty(unitRef))
                        fact.Unit = Units.Contains(unitRef)
                            ? Units[unitRef]
                            : throw new KeyNotFoundException($"Referenced unit '{unitRef}' does not exist");
                }

                if (fact.Facts.Any())
                    SetUnitReferences(fact.Facts);
            }
        }

        void UpdateContextNamespaces(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            foreach (var context in contextsWithMembers)
            {
                for (int i = 0; i < context.Scenario.ExplicitMembers.Count; i++)
                {
                    var m = context.Scenario.ExplicitMembers[i];

                    if (string.IsNullOrEmpty(m.Dimension.Namespace))
                        m.Dimension = new XmlQualifiedName(m.Dimension.Name, DimensionNamespace);

                    if (string.IsNullOrEmpty(m.Value.Namespace))
                    {
                        var ns = namespaces.LookupNamespace(m.Value.Name.Substring(0, m.Value.Name.IndexOf(':')));
                        var localname = m.Value.Name.Substring(m.Value.Name.IndexOf(':') + 1);
                        m.Value = new XmlQualifiedName(localname, ns);
                    }
                    context.Scenario.ExplicitMembers[i] = m;
                }

                for (int i = 0; i < context.Scenario.TypedMembers.Count; i++)
                {
                    var m = context.Scenario.TypedMembers[i];

                    if (string.IsNullOrEmpty(m.Dimension.Namespace))
                        m.Dimension = new XmlQualifiedName(m.Dimension.Name, DimensionNamespace);

                    if (string.IsNullOrEmpty(m.Domain.Namespace))
                        m.Domain = new XmlQualifiedName(m.Domain.Name, TypedDomainNamespace);

                    context.Scenario.TypedMembers[i] = m;
                }
            }
        }

        void GetFactNamespace()
        {
            if (string.IsNullOrEmpty(FactNamespace))
            {
                var fact = Facts.FirstOrDefault();
                if (fact != null)
                {
                    var ns = fact.Metric.Namespace;
                    if (string.IsNullOrEmpty(ns))
                    {
                        var idx = fact.Metric.Name.IndexOf(':');
                        if (idx != -1)
                        {
                            var prefix = fact.Metric.Name.Substring(idx);
                            ns = Namespaces.LookupNamespace(prefix);
                        }
                    }
                    FactNamespace = ns;
                }
            }
        }

        void DimensionFromTypedMembers(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            var contextWithTypedMembers = contextsWithMembers.FirstOrDefault(c => c.Scenario.TypedMembers.Any());
            if (contextWithTypedMembers != null)
            {
                var member = contextWithTypedMembers.Scenario.TypedMembers.First();
                var typedDomainNs = member.Domain.Namespace;

                if (string.IsNullOrEmpty(typedDomainNs))
                    typedDomainNs = namespaces.LookupNamespace(member.Domain.Name.Substring(0, member.Domain.Name.IndexOf(':')));

                TypedDomainNamespace = typedDomainNs;
                var dimensionNs = member.Dimension.Namespace;

                if (string.IsNullOrEmpty(dimensionNs))
                    dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));

                DimensionNamespace = dimensionNs;
            }
        }

        void DimensionFromExplicitMembers(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            if (string.IsNullOrEmpty(DimensionNamespace))
            {
                var contextWithExplicitMembers = contextsWithMembers.FirstOrDefault(c => c.Scenario.ExplicitMembers.Any());
                if (contextWithExplicitMembers != null)
                {
                    var member = contextWithExplicitMembers.Scenario.ExplicitMembers.First();
                    var dimensionNs = member.Dimension.Namespace;

                    if (string.IsNullOrEmpty(dimensionNs))
                        dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));

                    DimensionNamespace = dimensionNs;
                }
            }
        }

        public FilingIndicator AddFilingIndicator(string value)
        => AddFilingIndicator(value, true);

        public FilingIndicator AddFilingIndicator(Context context, string value)
        => AddFilingIndicator(context, value, true);

        public FilingIndicator AddFilingIndicator(string value, bool filed)
        => AddFilingIndicator(GetContext((Scenario)null), value, filed);

        public FilingIndicator AddFilingIndicator(Context context, string value, bool filed)
        => FilingIndicators.Add(context, value, filed);

        public Fact AddFact(Context context, string metric, string unitRef, string decimals, string value)
        => Facts.Add(context, metric, unitRef, decimals, value);

        public Fact AddFact(Scenario scenario, string metric, string unitRef, string decimals, string value)
        {
            if (scenario != null)
            {
                scenario.Instance = this;

                if (!scenario.HasMembers)
                    scenario = null;
            }
            return Facts.Add(scenario, metric, unitRef, decimals, value);
        }

        public Fact AddFact(Segment segment, string metric, string unitRef, string decimals, string value)
        {
            if (segment != null)
            {
                segment.Instance = this;

                if (!segment.HasMembers)
                    segment = null;
            }
            Facts.Instance = this;
            return Facts.Add(segment, metric, unitRef, decimals, value);
        }

        internal List<string> GetUsedDomainNamespaces()
        {
            var used = Contexts.
                Where(c => c != null).
                Where(c => c.Scenario != null).
                Where(c => c.Scenario.ExplicitMembers.Any()).
                SelectMany(c => c.Scenario.ExplicitMembers).
                Select(e => e.Value.Namespace).
                Distinct().
                ToList();

            GetUsedFactDomainNamespaces(used, Facts);
            return used;
        }

        internal void GetUsedFactDomainNamespaces(List<string> used, FactCollection facts)
        {
            var factNamespaces = new HashSet<string>();

            foreach (var fact in facts.Where(f => !string.IsNullOrEmpty(f.Value) || f.Facts.Count > 0))
            {
                if (fact.Value.Contains(':'))
                {
                    var prefix = fact.Value.Split(':')[0];
                    var ns = Namespaces.LookupNamespace(prefix);
                    if (ns != null)
                        factNamespaces.Add(ns);
                }
                else
                {
                    var prefix = fact.Metric.Name.Split(':')[0];
                    var ns = Namespaces.LookupNamespace(prefix);
                    if (ns != null)
                        factNamespaces.Add(ns);

                    GetUsedFactDomainNamespaces(used, fact.Facts);
                }
            }

            used.AddRange(factNamespaces);
        }

        #region serialization

        static XmlSerializer Serializer = new XmlSerializer(typeof(Instance));

        static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
        {
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = false,
            IgnoreComments = false,
            XmlResolver = null,
            ValidationType = ValidationType.None
        };

        static XmlWriterSettings XmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            Encoding = Encoding.UTF8
        };

        static InstanceInfo GetInstanceInfo(Stream stream)
        {
            string taxonomyVersion = null;
            string instanceGenerator = null;
            var comments = new List<string>();
            using (var reader = XmlReader.Create(stream, XmlReaderSettings))
            {
                var content = false;
                do
                {
                    reader.Read();
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.XmlDeclaration:
                            // skip
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            string value = reader.Value;
                            switch (reader.Name)
                            {
                                case "taxonomy-version":
                                    taxonomyVersion = value;
                                    break;
                                case "instance-generator":
                                    instanceGenerator = value;
                                    break;
                            }
                            break;
                        case XmlNodeType.Comment:
                            comments.Add(reader.Value);
                            break;
                        default:
                            // go read the actual document
                            content = true;
                            break;
                    }
                }
                while (!content);
            }
            return new InstanceInfo(taxonomyVersion, instanceGenerator, comments);
        }

        static void SetInstanceInfo(Instance instance, InstanceInfo info)
        {
            if (!string.IsNullOrEmpty(info.TaxonomyVersion))
                instance.TaxonomyVersion = info.TaxonomyVersion;

            if (!string.IsNullOrEmpty(info.InstanceGenerator))
                instance.InstanceGenerator = info.InstanceGenerator;

            instance.Comments = new Collection<string>(info.Comments);
        }

        static void CleanupAfterDeserialization(Instance instance, InstanceOptions options)
        {
            instance.SetContextReferences(instance.Facts);
            instance.SetUnitReferences(instance.Facts);

            if (options.HasFlag(InstanceOptions.CollapseDuplicateContexts))
                instance.CollapseDuplicateContexts();

            if (options.HasFlag(InstanceOptions.RemoveUnusedObjects))
                instance.RemoveUnusedObjects();

            instance.RebuildNamespacesAfterRead();
            instance.SetInstanceReferences();
        }

        void SetInstanceReferences()
        {
            foreach (var context in Contexts)
            {
                var s = context.Scenario;
                if (s != null)
                    if (s.Instance == null)
                        s.Instance = this;
            }

            if (Entity != null)
            {
                var seg = Entity.Segment;
                if (seg != null)
                    if (seg.Instance == null)
                        seg.Instance = this;
            }
        }

        XmlSerializerNamespaces GetXmlSerializerNamespaces()
        {
            var usedDomains = GetUsedDomainNamespaces();

            var result = new XmlSerializerNamespaces();

            foreach (var item in DefaultXbrlNamespaces)
                result.Add(item.Key, item.Value);

            foreach (var item in DefaultUnitNamespaces)
                if (Units.Any(u => u.Measure.StartsWith(item.Key)))
                {
                    result.Add(item.Key, item.Value);
                }
                else
                {
                    Namespaces.RemoveNamespace(item.Key, item.Value);
                    Console.WriteLine($"Namespace removed: {item.Key}:{item.Value}");
                }

            if (FilingIndicators.Any())
                foreach (var item in DefaultIndicatorNamespaces)
                    result.Add(item.Key, item.Value);

            var namespaces = new List<string>();

            if (Facts.Any())
                namespaces.Add(FactNamespace);

            var scenarios = Contexts.Where(c => c.Scenario != null).Select(c => c.Scenario).ToList();

            if (scenarios.Any(s => s.TypedMembers.Any()))
            {
                namespaces.Add(DimensionNamespace);
                namespaces.Add(TypedDomainNamespace);
            }
            else if (scenarios.Any(s => s.ExplicitMembers.Any()))
            {
                namespaces.Add(DimensionNamespace);
            }

            var segments = Contexts.Where(c => c.Entity.Segment != null).Select(c => c.Entity.Segment).ToList();

            if (segments.Any(s => s.TypedMembers.Any()))
            {
                namespaces.Add(DimensionNamespace);
                namespaces.Add(TypedDomainNamespace);
            }
            else if (segments.Any(s => s.ExplicitMembers.Any()))
            {
                namespaces.Add(DimensionNamespace);
            }

            namespaces.
                Where(i => !string.IsNullOrEmpty(i)).
                Concat(usedDomains).
                ToList().
                ForEach(i => result.Add(Namespaces.LookupPrefix(i), i));

            return result;
        }

        public static Instance FromStream(Stream stream)
        => FromStream(stream, true);

        public static Instance FromStream(Stream stream, bool removeUnusedObjects)
        => FromStream(stream, removeUnusedObjects, true);

        public static Instance FromStream(Stream stream, bool removeUnusedObjects, bool collapseDuplicateContexts)
        {
            stream.Position = 0;

            var info = GetInstanceInfo(stream);

            stream.Position = 0;

            var xbrl = (Instance)Serializer.Deserialize(stream);

            var options = InstanceOptions.None;
            if (removeUnusedObjects)
                options |= InstanceOptions.RemoveUnusedObjects;

            if (collapseDuplicateContexts)
                options |= InstanceOptions.CollapseDuplicateContexts;

            CleanupAfterDeserialization(xbrl, options);

            SetInstanceInfo(xbrl, info);

            return xbrl;

        }

        public void ToStream(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream, XmlWriterSettings))
                ToXmlWriter(writer);
        }

        public static Instance FromFile(string path)
        => FromFile(path, false);

        public static Instance FromFile(string path, bool removeUnusedObjects)
        {
            Instance xbrl;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                xbrl = FromStream(stream, removeUnusedObjects);

            return xbrl;
        }

        public void ToFile(string path)
        {
            using (var writer = XmlWriter.Create(path, XmlWriterSettings))
                ToXmlWriter(writer);
        }

        void ToXmlWriter(XmlWriter writer)
        {
            var ns = GetXmlSerializerNamespaces();

            var info = $"id=\"{id}\" version=\"{version}\" creationdate=\"{DateTime.Now:yyyy-MM-ddTHH:mm:ss:ffzzz}\"";

            writer.WriteProcessingInstruction("instance-generator", info);

            if (!string.IsNullOrEmpty(TaxonomyVersion))
                writer.WriteProcessingInstruction("taxonomy-version", TaxonomyVersion);

            foreach (var item in Comments)
                writer.WriteComment(item);

            Serializer.Serialize(writer, this, ns);
        }

        public XmlDocument ToXmlDocument()
        {
            var document = new XmlDocument();
            var declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declaration);
            var nav = document.CreateNavigator();

            using (var writer = nav.AppendChild())
                ToXmlWriter(writer);

            return document;
        }

        public static Instance FromXml(string content)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
                writer.Flush();

                stream.Position = 0;

                return FromStream(stream);
            }
        }

        public string ToXml()
        => ToXmlDocument().OuterXml;

        #endregion
    }
}