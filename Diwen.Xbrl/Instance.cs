//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections.ObjectModel;

    [Serializable]
    [XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Instance : IEquatable<Instance>
    {
        static IFormatProvider ic = CultureInfo.InvariantCulture;
        static AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        static Version version = assembly.Version;
        static string id = assembly.Name;

        internal static Dictionary<string, string> DefaultNamespaces = new Dictionary<string, string> {
            { "xsi", "http://www.w3.org/2001/XMLSchema-instance" },
            { "xbrli", "http://www.xbrl.org/2003/instance" },
            { "link", "http://www.xbrl.org/2003/linkbase" },
            { "xlink", "http://www.w3.org/1999/xlink" },
            { "iso4217", "http://www.xbrl.org/2003/iso4217" },
            { "find", "http://www.eurofiling.info/xbrl/ext/filing-indicators" },
            { "xbrldi", "http://xbrl.org/2006/xbrldi" },
        };

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces XmlSerializerNamespaces { get; set; }

        [XmlIgnore]
        public XmlNamespaceManager Namespaces { get; set; }

        [XmlIgnore]
        public Entity Entity { get; set; }

        [XmlIgnore]
        public Period Period { get; set; }

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
        public SchemaReference SchemaReference { get; set; }

        [XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
        public UnitCollection Units { get; set; }

        [XmlArray("fIndicators", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        [XmlArrayItem("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        public FilingIndicatorCollection FilingIndicators { get; set; }

        [XmlElement("context", Namespace = "http://www.xbrl.org/2003/instance")]
        public ContextCollection Contexts { get; set; }

        [XmlIgnore]
        public FactCollection Facts { get; private set; }

        [XmlAnyElement]
        public XmlElement[] FactItems
        {
            get
            {
                var elements = new List<XmlElement>();
                foreach(var item in Facts)
                {
                    elements.Add(item.ToXmlElement());
                }
                return elements.ToArray();
            }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("value");
                }
                foreach(var element in value)
                {
                    Facts.Add(Fact.FromXmlElement(element));
                }
            }
        }

        [XmlIgnore]
        public Collection<string> Comments { get; private set; }

        [XmlIgnore]
        public bool CheckExplicitMemberDomainExists { get; set; }

        public void SetDimensionNamespace(string prefix, Uri namespaceUri)
        {
            if(namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            SetDimensionNamespace(prefix, namespaceUri.ToString());
        }

        public void SetDimensionNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
            DimensionNamespace = namespaceUri;
        }

        public void SetMetricNamespace(string prefix, Uri namespaceUri)
        {
            if(namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            SetMetricNamespace(prefix, namespaceUri.ToString());
        }

        public void SetMetricNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
            FactNamespace = namespaceUri;
        }

        public void SetTypedDomainNamespace(string prefix, Uri namespaceUri)
        {
            if(namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            SetTypedDomainNamespace(prefix, namespaceUri.ToString());
        }

        public void SetTypedDomainNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
            TypedDomainNamespace = namespaceUri;
        }

        public void AddDomainNamespace(string prefix, Uri namespaceUri)
        {
            if(namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            AddDomainNamespace(prefix, namespaceUri.ToString());
        }

        public void AddDomainNamespace(string prefix, string namespaceUri)
        {
            Namespaces.AddNamespace(prefix, namespaceUri);
        }

        public Context GetContext(Scenario scenario)
        {
            Context context;

            context = scenario == null 
                ? Contexts.FirstOrDefault(c => c.Scenario == null) 
                : Contexts.FirstOrDefault(c => scenario.Equals(c.Scenario));

            if(context == null)
            {
                context = new Context(scenario);
                Contexts.Add(context);
            }

            return context;
        }

        void AddDefaultNamespaces()
        {
            foreach(var item in DefaultNamespaces)
            {
                Namespaces.AddNamespace(item.Key, item.Value);
            }
        }

        public void RemoveUnusedUnits()
        {
            var used = Facts.
                Where(f => f.Unit != null).
                Select(f => f.Unit.Id).
                Distinct().
                ToList();

            for(int i = 0; i < Units.Count; i++)
            {
                var u = Units[i];
                if(!used.Contains(u.Id))
                {
                    Units[i] = null;
                }
            }

            Unit nullUnit = null;
            while(Units.Remove(nullUnit))
            {
            }
        }

        public void RemoveUnusedObjects()
        {
            RemoveUnusedUnits();
            RemoveUnusedContexts();
        }

        public void CollapseDuplicateContexts()
        {
            var found = false;
            for(int i = 0; i < Contexts.Count; i++)
            {
                var left = Contexts[i];
                for(int j = i + 1; j < Contexts.Count; j++)
                {
                    var right = Contexts[j];
                    if(left.Equals(right))
                    {
                        found = true;
                        foreach(var fact in Facts.Where(f => f.Context.Id == right.Id))
                        {
                            fact.Context = left;
                        }
                    }
                }
            }
            if(found)
            {
                RemoveUnusedContexts();
            }
        }

        public void RemoveUnusedContexts()
        {
            var used = Facts.
                Where(f => f.Context != null).
                Select(f => f.Context.Id).
                Concat(FilingIndicators.Select(f => f.ContextRef)).
                Distinct().
                ToList();

            for(int i = 0; i < Contexts.Count; i++)
            {
                var c = Contexts[i];
                if(!used.Contains(c.Id))
                {
                    Contexts[i] = null;
                }
            }

            Context nullContext = null;
            while(Contexts.Remove(nullContext))
            {
            }
        }

        public Instance()
        {
            Namespaces = new XmlNamespaceManager(new NameTable());
            AddDefaultNamespaces();
            SchemaReference = new SchemaReference();
            FilingIndicators = new FilingIndicatorCollection();
            Units = new UnitCollection(this);
            Contexts = new ContextCollection(this);
            Facts = new FactCollection(this);
            Comments = new Collection<string>();
        }

        #region IEquatable implementation

        public bool Equals(Instance other)
        {
            var result = false;
            if(other != null)
            {
                if(SchemaReference.Equals(other.SchemaReference))
                {
                    if(Units.Equals(other.Units))
                    {
                        if(FilingIndicators.Equals(other.FilingIndicators))
                        {
                            if(Contexts.Equals(other.Contexts))
                            {
                                result |= Facts.Equals(other.Facts);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            return SchemaReference.GetHashCode()
            ^ Units.GetHashCode()
            ^ FilingIndicators.GetHashCode()
            ^ Contexts.GetHashCode()
            ^ Facts.GetHashCode();
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as Instance;
            return other != null && Equals(other);
        }

        void RebuildNamespacesAfterRead()
        {
            var namespaces = new XmlNamespaceManager(new NameTable());
            foreach(var item in XmlSerializerNamespaces.ToArray())
            {
                namespaces.AddNamespace(item.Name, item.Namespace);
            }

            var contextsWithMembers = Contexts.Where(c => c.Scenario != null);
            DimensionFromTypedMembers(namespaces, contextsWithMembers);
            DimensionFromExplicitMembers(namespaces, contextsWithMembers);

            GetFactNamespace();

            UpdateContextNamespaces(namespaces, contextsWithMembers);

            Namespaces = namespaces;
        }

        void SetContextReferences()
        {
            foreach(var filingIndicator in FilingIndicators)
            {
                if(filingIndicator.Context == null)
                {
                    var contextRef = filingIndicator.ContextRef;
                    if(!string.IsNullOrEmpty(contextRef))
                    {
                        if(!Contexts.Contains(contextRef))
                        {
                            throw new KeyNotFoundException(string.Format(ic, "Referenced context '{0}' does not exist", contextRef));
                        }
                        filingIndicator.Context = Contexts[contextRef];
                    }
                }
            }

            foreach(var fact in Facts)
            {
                if(fact.Context == null)
                {
                    var contextRef = fact.ContextRef;
                    if(!string.IsNullOrEmpty(contextRef))
                    {
                        if(!Contexts.Contains(contextRef))
                        {
                            throw new KeyNotFoundException(string.Format(ic, "Referenced context '{0}' does not exist", contextRef));
                        }
                        fact.Context = Contexts[contextRef];
                    }
                }
            }
        }

        void SetUnitReferences()
        {
            foreach(var fact in Facts)
            {
                if(fact.Unit == null)
                {
                    var unitRef = fact.UnitRef;
                    if(!string.IsNullOrEmpty(unitRef))
                    {
                        if(!Units.Contains(unitRef))
                        {
                            throw new KeyNotFoundException(string.Format(ic, "Referenced unit '{0}' does not exist", unitRef));
                        }
                        fact.Unit = Units[unitRef];
                    }
                }
            }
        }

        void UpdateContextNamespaces(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            foreach(var context in contextsWithMembers)
            {
                foreach(var m in context.Scenario.ExplicitMembers)
                {
                    if(string.IsNullOrEmpty(m.Dimension.Namespace))
                    {
                        m.Dimension = new XmlQualifiedName(m.Dimension.Name, DimensionNamespace);
                    }
                    if(string.IsNullOrEmpty(m.Value.Namespace))
                    {
                        var ns = namespaces.LookupNamespace(m.Value.Name.Substring(0, m.Value.Name.IndexOf(':')));
                        var localname = m.Value.Name.Substring(m.Value.Name.IndexOf(':') + 1);
                        m.Value = new XmlQualifiedName(localname, ns);
                    }
                }
                foreach(var m in context.Scenario.TypedMembers)
                {
                    if(string.IsNullOrEmpty(m.Dimension.Namespace))
                    {
                        m.Dimension = new XmlQualifiedName(m.Dimension.Name, DimensionNamespace);
                    }
                    if(string.IsNullOrEmpty(m.Domain.Namespace))
                    {
                        m.Domain = new XmlQualifiedName(m.Domain.Name, TypedDomainNamespace);
                    }
                }
            }
        }

        void GetFactNamespace()
        {
            if(string.IsNullOrEmpty(FactNamespace))
            {
                var fact = Facts.FirstOrDefault();
                if(fact != null)
                {
                    var ns = fact.Metric.Namespace;
                    if(string.IsNullOrEmpty(ns))
                    {
                        var idx = fact.Metric.Name.IndexOf(':');
                        if(idx != -1)
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
            var contextWithTypedMembers = contextsWithMembers.FirstOrDefault(c => c.Scenario.TypedMembers != null && c.Scenario.TypedMembers.Count != 0);
            if(contextWithTypedMembers != null)
            {
                var member = contextWithTypedMembers.Scenario.TypedMembers.First();
                var typedDomainNs = member.Domain.Namespace;
                if(string.IsNullOrEmpty(typedDomainNs))
                {
                    typedDomainNs = namespaces.LookupNamespace(member.Domain.Name.Substring(0, member.Domain.Name.IndexOf(':')));
                }
                TypedDomainNamespace = typedDomainNs;
                var dimensionNs = member.Dimension.Namespace;
                if(string.IsNullOrEmpty(dimensionNs))
                {
                    dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));
                }
                DimensionNamespace = dimensionNs;
            }
        }

        void DimensionFromExplicitMembers(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            if(string.IsNullOrEmpty(DimensionNamespace))
            {
                var contextWithExplicitMembers = contextsWithMembers.FirstOrDefault(c => c.Scenario.ExplicitMembers != null && c.Scenario.ExplicitMembers.Count != 0);
                if(contextWithExplicitMembers != null)
                {
                    var member = contextWithExplicitMembers.Scenario.ExplicitMembers.First();
                    var dimensionNs = member.Dimension.Namespace;
                    if(string.IsNullOrEmpty(dimensionNs))
                    {
                        dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));
                    }
                    DimensionNamespace = dimensionNs;
                }
            }
        }

        public FilingIndicator AddFilingIndicator(string value)
        {
            return AddFilingIndicator(value, true);
        }

        public FilingIndicator AddFilingIndicator(Context context, string value)
        {
            return AddFilingIndicator(context, value, true);
        }

        public FilingIndicator AddFilingIndicator(string value, bool filed)
        {
            var context = GetContext(null);
            return AddFilingIndicator(context, value, filed);
        }

        public FilingIndicator AddFilingIndicator(Context context, string value, bool filed)
        {
            return FilingIndicators.Add(context, value, filed);
        }

        public Fact AddFact(Context context, string metric, string unitRef, string decimals, string value)
        {
            return Facts.Add(context, metric, unitRef, decimals, value);
        }

        public Fact AddFact(Scenario scenario, string metric, string unitRef, string decimals, string value)
        {
            if(scenario != null)
            {
                scenario.Instance = this;

                if(scenario.ExplicitMembers.Count == 0 && scenario.TypedMembers.Count == 0)
                {
                    scenario = null;
                }
            }
            return Facts.Add(scenario, metric, unitRef, decimals, value);
        }

        internal List<string> GetUsedDomainNamespaces()
        {
            var used = new List<string>();
            var contexts = Contexts.Where(c => c != null && c.Scenario != null && c.Scenario.ExplicitMembers != null).ToList();
            used.AddRange(contexts.SelectMany(c => c.Scenario.ExplicitMembers).Select(e => e.Value.Namespace).Distinct());

            var factNamespaces = new HashSet<string>();
            var facts = Facts.Where(f => !string.IsNullOrEmpty(f.Value) && f.Value.Contains(":"));
            foreach(var fact in facts)
            {
                var prefix = fact.Value.Split(':')[0];
                var ns = Namespaces.LookupNamespace(prefix);
                if(ns != null)
                {
                    factNamespaces.Add(ns);
                }
            }
            used.AddRange(factNamespaces);
            return used;
        }

        #region serialization

        static XmlSerializer Serializer = new XmlSerializer(typeof(Instance));

        static XmlWriterSettings XmlWriterSettings = new XmlWriterSettings {
            Indent = true,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            Encoding = Encoding.UTF8
        };

        public static Instance FromStream(Stream stream, bool removeUnusedObjects = false)
        {
            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = false;
            settings.IgnoreComments = false;
            var reader = XmlReader.Create(stream, settings);
            var content = false;
            string taxonomyVersion = null;
            string instanceGenerator = null;
            var comments = new List<string>();

            do
            {
                reader.Read();
                switch(reader.NodeType)
                {
                case XmlNodeType.XmlDeclaration:
					// skip
                    break;
                case XmlNodeType.ProcessingInstruction:

                    switch(reader.Name)
                    {
                    case "taxonomy-version":
                        taxonomyVersion = reader.Value;
                        break;
                    case "instance-generator":
                        instanceGenerator = reader.Value;
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

            stream.Position = 0;

            var xbrl = (Instance)Serializer.Deserialize(stream);

            xbrl.SetContextReferences();
            xbrl.SetUnitReferences();
            if(removeUnusedObjects)
            {
                xbrl.RemoveUnusedObjects();
            }
            xbrl.RebuildNamespacesAfterRead();
            xbrl.SetInstanceReferences();

            if(!string.IsNullOrEmpty(taxonomyVersion))
            {
                xbrl.TaxonomyVersion = taxonomyVersion;
            }

            if(!string.IsNullOrEmpty(instanceGenerator))
            {
                xbrl.InstanceGenerator = instanceGenerator;
            }

            xbrl.Comments = new Collection<string>(comments);

            return xbrl;
        }

        void SetInstanceReferences()
        {
            foreach(var context in Contexts)
            {
                var s = context.Scenario;
                if(s != null)
                {
                    if(s.Instance == null)
                    {
                        s.Instance = this;
                    }
                }
            }
        }

        public void ToStream(Stream stream)
        {
            using(var writer = XmlWriter.Create(stream, XmlWriterSettings))
            {
                ToXmlWriter(writer);
            }
        }

        public static Instance FromFile(string path, bool removeUnusedObjects = false)
        {
            Instance xbrl;

            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                xbrl = Instance.FromStream(stream, removeUnusedObjects);
            }

            return xbrl;
        }

        public void ToFile(string path)
        {
            using(var writer = XmlWriter.Create(path, XmlWriterSettings))
            {
                ToXmlWriter(writer);
            }
        }

        void ToXmlWriter(XmlWriter writer)
        {
            var ns = this.ToXmlSerializerNamespaces();

            var info = string.Format("id='{0}' version='{1}' creationdate='{2:yyyy-MM-ddTHH:mm:ss:ffzzz}", id, version, DateTime.Now);

            writer.WriteProcessingInstruction("instance-generator", info);

            if(!string.IsNullOrEmpty(TaxonomyVersion))
            {
                writer.WriteProcessingInstruction("taxonomy-version", TaxonomyVersion);
            }

            foreach(var item in Comments)
            {
                writer.WriteComment(item);
            }

            Serializer.Serialize(writer, this, ns);
        }

        public XmlDocument ToXmlDocument()
        {
            var document = new XmlDocument();
            var declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declaration);
            var nav = document.CreateNavigator();
            using(var writer = nav.AppendChild())
            {
                ToXmlWriter(writer);
            }
            return document;
        }

        #endregion
    }
}