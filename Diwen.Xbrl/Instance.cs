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

    [Serializable]
    [XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Instance : IEquatable<Instance>
    {
        private static IFormatProvider ic = CultureInfo.InvariantCulture;
        private static AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        private static Version version = assembly.Version;
        private static string id = assembly.Name;

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
        public string FactNamespace { get; private set; }

        [XmlIgnore]
        public string DimensionNamespace { get; private set; }

        [XmlIgnore]
        public string TypedDomainNamespace { get; private set; }

        [XmlElement("schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
        public SchemaReference SchemaReference { get; set; }

        [XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
        public UnitCollection Units { get; private set; }

        [XmlArray("fIndicators", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        [XmlArrayItem("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        public FilingIndicatorCollection FilingIndicators { get; private set; }

        [XmlElement("context", Namespace = "http://www.xbrl.org/2003/instance")]
        public ContextCollection Contexts { get; private set; }

        [XmlIgnore]
        public FactCollection Facts { get; private set; }

        [XmlAnyElement]
        public XmlElement[] FactItems
        {
            get
            {
                var elements = new List<XmlElement>();
                foreach (var item in Facts)
                {
                    elements.Add(item.ToXmlElement());
                }
                return elements.ToArray();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                foreach (var element in value)
                {
                    this.Facts.Add(Fact.FromXmlElement(element));
                }
            }
        }

        [XmlIgnore]
        public bool CheckExplicitMemberDomainExists { get; set; }

        public void SetDimensionNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            SetDimensionNamespace(prefix, namespaceUri.ToString());
        }

        public void SetDimensionNamespace(string prefix, string namespaceUri)
        {
            this.Namespaces.AddNamespace(prefix, namespaceUri);
            this.DimensionNamespace = namespaceUri;
        }

        public void SetMetricNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            SetMetricNamespace(prefix, namespaceUri.ToString());
        }

        public void SetMetricNamespace(string prefix, string namespaceUri)
        {
            this.Namespaces.AddNamespace(prefix, namespaceUri);
            this.FactNamespace = namespaceUri;
        }

        public void SetTypedDomainNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            SetTypedDomainNamespace(prefix, namespaceUri.ToString());
        }

        public void SetTypedDomainNamespace(string prefix, string namespaceUri)
        {
            this.Namespaces.AddNamespace(prefix, namespaceUri);
            this.TypedDomainNamespace = namespaceUri;
        }

        public void AddDomainNamespace(string prefix, Uri namespaceUri)
        {
            if (namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            AddDomainNamespace(prefix, namespaceUri.ToString());
        }

        public void AddDomainNamespace(string prefix, string namespaceUri)
        {
            this.Namespaces.AddNamespace(prefix, namespaceUri);
        }

        public Context GetContext(Scenario scenario)
        {
            Context context = null;

            if (scenario == null)
            {
                context = this.Contexts.FirstOrDefault(c => c.Scenario == null);
            }
            else
            {
                context = this.Contexts.FirstOrDefault(c => scenario.Equals(c.Scenario));
            }

            if (context == null)
            {
                context = new Context(scenario);
                Contexts.Add(context);
            }

            return context;
        }

        private void AddDefaultNamespaces()
        {
            foreach (var item in DefaultNamespaces)
            {
                Namespaces.AddNamespace(item.Key, item.Value);
            }
        }

        public void RemoveUnusedUnits()
        {
            var used = this.Facts.Where(f => f.Unit != null).Select(f => f.Unit.Id).Distinct();

            for (int i = 0; i < this.Units.Count; i++)
            {
                var u = this.Units[i];
                if (!used.Contains(u.Id))
                {
                    this.Units[i] = null;
                }
            }

            Unit nullUnit = null;
            while (this.Units.Remove(nullUnit))
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
            for (int i = 0; i < this.Contexts.Count; i++)
            {
                var left = this.Contexts[i];
                for (int j = i + 1; j < this.Contexts.Count; j++)
                {
                    var right = this.Contexts[j];
                    if (left.Equals(right))
                    {
                        found = true;
                        foreach (var fact in this.Facts.Where(f => f.Context.Id == right.Id))
                        {
                            fact.Context = left;
                        }
                    }
                }
            }
            if (found)
            {
                this.RemoveUnusedContexts();
            }
        }

        public void RemoveUnusedContexts()
        {
            var used = this.Facts.Where(f => f.Context != null).Select(f => f.Context.Id).Concat(this.FilingIndicators.Select(f => f.ContextRef)).Distinct();


            for (int i = 0; i < this.Contexts.Count; i++)
            {
                var c = this.Contexts[i];
                if (!used.Contains(c.Id))
                {
                    this.Contexts[i] = null;
                }
            }

            Context nullContext = null;
            while (this.Contexts.Remove(nullContext))
            {
            }
        }

        public Instance()
        {
            this.Namespaces = new XmlNamespaceManager(new NameTable());
            AddDefaultNamespaces();
            this.SchemaReference = new SchemaReference();
            this.FilingIndicators = new FilingIndicatorCollection();
            this.Units = new UnitCollection(this);
            this.Contexts = new ContextCollection(this);
            this.Facts = new FactCollection(this);
        }

        #region IEquatable implementation

        public bool Equals(Instance other)
        {
            var result = false;
            if (other != null)
            {
                if (this.SchemaReference.Equals(other.SchemaReference))
                {
                    if (this.Units.Equals(other.Units))
                    {
                        if (this.FilingIndicators.Equals(other.FilingIndicators))
                        {
                            if (this.Contexts.Equals(other.Contexts))
                            {
                                if (this.Facts.Equals(other.Facts))
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            return this.SchemaReference.GetHashCode()
            ^ this.Units.GetHashCode()
            ^ this.FilingIndicators.GetHashCode()
            ^ this.Contexts.GetHashCode()
            ^ this.Facts.GetHashCode();
        }

        #endregion

        public override bool Equals(object obj)
        {
            var other = obj as Instance;
            if (other != null)
            {
                return this.Equals(other);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        private void RebuildNamespacesAfterRead()
        {
            var namespaces = new XmlNamespaceManager(new NameTable());
            foreach (var item in this.XmlSerializerNamespaces.ToArray())
            {
                namespaces.AddNamespace(item.Name, item.Namespace);
            }

            var contextsWithMembers = this.Contexts.Where(c => c.Scenario != null);
            DimensionFromTypedMembers(namespaces, contextsWithMembers);
            DimensionFromExplicitMembers(namespaces, contextsWithMembers);

            GetFactNamespace();

            UpdateContextNamespaces(namespaces, contextsWithMembers);

            this.Namespaces = namespaces;
        }

        private void SetContextReferences()
        {
            foreach (var filingIndicator in this.FilingIndicators)
            {
                if (filingIndicator.Context == null)
                {
                    var contextRef = filingIndicator.ContextRef;
                    if (!string.IsNullOrEmpty(contextRef))
                    {
                        if (!this.Contexts.Contains(contextRef))
                        {
                            throw new KeyNotFoundException(string.Format(ic, "Referenced context '{0}' does not exist", contextRef));
                        }
                        filingIndicator.Context = this.Contexts[contextRef];
                    }
                }
            }

            foreach (var fact in this.Facts)
            {
                if (fact.Context == null)
                {
                    var contextRef = fact.ContextRef;
                    if (!string.IsNullOrEmpty(contextRef))
                    {
                        if (!this.Contexts.Contains(contextRef))
                        {
                            throw new KeyNotFoundException(string.Format(ic, "Referenced context '{0}' does not exist", contextRef));
                        }
                        fact.Context = this.Contexts[contextRef];
                    }
                }
            }
        }

        private void SetUnitReferences()
        {
            foreach (var fact in this.Facts)
            {
                if (fact.Unit == null)
                {
                    var unitRef = fact.UnitRef;
                    if (!string.IsNullOrEmpty(unitRef))
                    {
                        if (!this.Units.Contains(unitRef))
                        {
                            throw new KeyNotFoundException(string.Format(ic, "Referenced unit '{0}' does not exist", unitRef));
                        }
                        fact.Unit = this.Units[unitRef];
                    }
                }
            }
        }

        private void UpdateContextNamespaces(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            foreach (var context in contextsWithMembers)
            {
                foreach (var m in context.Scenario.ExplicitMembers)
                {
                    if (string.IsNullOrEmpty(m.Dimension.Namespace))
                    {
                        m.Dimension = new XmlQualifiedName(m.Dimension.Name, this.DimensionNamespace);
                    }
                    if (string.IsNullOrEmpty(m.Value.Namespace))
                    {
                        var ns = namespaces.LookupNamespace(m.Value.Name.Substring(0, m.Value.Name.IndexOf(':')));
                        var localname = m.Value.Name.Substring(m.Value.Name.IndexOf(':') + 1);
                        m.Value = new XmlQualifiedName(localname, ns);
                    }
                }
                foreach (var m in context.Scenario.TypedMembers)
                {
                    if (string.IsNullOrEmpty(m.Dimension.Namespace))
                    {
                        m.Dimension = new XmlQualifiedName(m.Dimension.Name, this.DimensionNamespace);
                    }
                    if (string.IsNullOrEmpty(m.Domain.Namespace))
                    {
                        m.Domain = new XmlQualifiedName(m.Domain.Name, this.TypedDomainNamespace);
                    }
                }
            }
        }

        private void GetFactNamespace()
        {
            if (string.IsNullOrEmpty(this.FactNamespace))
            {
                var fact = this.Facts.FirstOrDefault();
                if (fact != null)
                {
                    var ns = fact.Metric.Namespace;
                    if (string.IsNullOrEmpty(ns))
                    {
                        var idx = fact.Metric.Name.IndexOf(':');
                        if (idx != -1)
                        {
                            var prefix = fact.Metric.Name.Substring(idx);
                            ns = this.Namespaces.LookupNamespace(prefix);
                        }
                    }
                    this.FactNamespace = ns;
                }
            }
        }

        private void DimensionFromTypedMembers(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            var contextWithTypedMembers = contextsWithMembers.FirstOrDefault(c => c.Scenario.TypedMembers != null && c.Scenario.TypedMembers.Count != 0);
            if (contextWithTypedMembers != null)
            {
                var member = contextWithTypedMembers.Scenario.TypedMembers.First();
                var typedDomainNs = member.Domain.Namespace;
                if (string.IsNullOrEmpty(typedDomainNs))
                {
                    typedDomainNs = namespaces.LookupNamespace(member.Domain.Name.Substring(0, member.Domain.Name.IndexOf(':')));
                }
                this.TypedDomainNamespace = typedDomainNs;
                var dimensionNs = member.Dimension.Namespace;
                if (string.IsNullOrEmpty(dimensionNs))
                {
                    dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));
                }
                this.DimensionNamespace = dimensionNs;
            }
        }

        private void DimensionFromExplicitMembers(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
        {
            if (string.IsNullOrEmpty(this.DimensionNamespace))
            {
                var contextWithExplicitMembers = contextsWithMembers.FirstOrDefault(c => c.Scenario.ExplicitMembers != null && c.Scenario.ExplicitMembers.Count != 0);
                if (contextWithExplicitMembers != null)
                {
                    var member = contextWithExplicitMembers.Scenario.ExplicitMembers.First();
                    var dimensionNs = member.Dimension.Namespace;
                    if (string.IsNullOrEmpty(dimensionNs))
                    {
                        dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));
                    }
                    this.DimensionNamespace = dimensionNs;
                }
            }
        }

        public FilingIndicator AddFilingIndicator(string value)
        {
            var context = this.GetContext(null);
            return AddFilingIndicator(context, value);
        }

        public FilingIndicator AddFilingIndicator(Context context, string value)
        {
            return this.FilingIndicators.Add(context, value);
        }

        public Fact AddFact(Context context, string metric, string unitRef, string decimals, string value)
        {
            return this.Facts.Add(context, metric, unitRef, decimals, value);
        }

        public Fact AddFact(Scenario scenario, string metric, string unitRef, string decimals, string value)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException("scenario");
            }
            scenario.Instance = this;
            return this.Facts.Add(scenario, metric, unitRef, decimals, value);
        }

        internal List<string> GetUsedDomainNamespaces()
        {
            var used = new List<string>();
            var contexts = this.Contexts.Where(c => c != null && c.Scenario != null && c.Scenario.ExplicitMembers != null).ToList();
            used.AddRange(contexts.SelectMany(c => c.Scenario.ExplicitMembers).Select(e => e.Value.Namespace).Distinct());
            return used;
        }

        #region serialization

        private static XmlSerializer Serializer = new XmlSerializer(typeof(Instance));

        private static XmlWriterSettings XmlWriterSettings = new XmlWriterSettings
        {
            Indent = true,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            Encoding = UTF8Encoding.UTF8
        };

        public static Instance FromStream(Stream stream, bool removeUnusedObjects = false)
        {
            var xbrl = (Instance)Serializer.Deserialize(stream);

            xbrl.SetContextReferences();
            xbrl.SetUnitReferences();


            if (removeUnusedObjects)
            {
                xbrl.RemoveUnusedObjects();
            }

            xbrl.RebuildNamespacesAfterRead();
            xbrl.SetInstanceReferences();
            return xbrl;
        }

        private void SetInstanceReferences()
        {
            foreach (var context in this.Contexts)
            {
                var s = context.Scenario;
                if (s != null)
                {
                    if (s.Instance == null)
                    {
                        s.Instance = this;
                    }
                }
            }
        }

        public void ToStream(Stream stream)
        {
            using (var writer = XmlWriter.Create(stream, XmlWriterSettings))
            {
                ToXmlWriter(writer);
            }
        }

        public static Instance FromFile(string path, bool removeUnusedObjects = false)
        {
            Instance xbrl = null;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                xbrl = Instance.FromStream(stream, removeUnusedObjects);
            }

            return xbrl;
        }

        public void ToFile(string path)
        {
            using (var writer = XmlWriter.Create(path, XmlWriterSettings))
            {
                ToXmlWriter(writer);
            }
        }

        private void ToXmlWriter(XmlWriter writer)
        {
            var ns = this.ToXmlSerializerNamespaces();

            var info = string.Format("id='{0}' version='{1}' creationdate='{2:yyyy-MM-ddTHH:mm:ss:ffzzz}", id, version, DateTime.Now);

            writer.WriteProcessingInstruction("instance-generator", info);

            if (!string.IsNullOrEmpty(this.TaxonomyVersion))
            {
                writer.WriteProcessingInstruction("taxonomy-version", this.TaxonomyVersion);
            }
            Serializer.Serialize(writer, this, ns);
        }

        public XmlDocument ToXmlDocument()
        {
            var document = new XmlDocument();
            var declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            document.AppendChild(declaration);
            var nav = document.CreateNavigator();
            using (var writer = nav.AppendChild())
            {
                ToXmlWriter(writer);
            }
            return document;
        }

        #endregion
    }
}