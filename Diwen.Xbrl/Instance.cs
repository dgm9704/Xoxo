namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Instance : IEquatable<Instance>
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces XmlSerializerNamespaces{ get; set; }

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

		[XmlIgnore]
		public UnitCollection Units { get; private set; }

		[XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
		public UnitCollection UsedUnits { get { return Units.UsedUnits(); } private set { } }

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
					this.Facts.Add(Fact.FromXmlElement(element));
				}
			}
		}

		[XmlIgnore]
		public bool CheckUnitExists { get; set; }

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
			this.Namespaces.AddNamespace(prefix, namespaceUri);
			this.DimensionNamespace = namespaceUri;
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
			this.Namespaces.AddNamespace(prefix, namespaceUri);
			this.FactNamespace = namespaceUri;
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
			this.Namespaces.AddNamespace(prefix, namespaceUri);
			this.TypedDomainNamespace = namespaceUri;
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
			this.Namespaces.AddNamespace(prefix, namespaceUri);
		}

		public Context GetContext(Scenario scenario)
		{
			Context context = null;

			if(scenario == null)
			{
				context = this.Contexts.FirstOrDefault(c => c.Scenario == null);
			}
			else
			{
				context = this.Contexts.FirstOrDefault(c => scenario.Equals(c.Scenario));
			}

			if(context == null)
			{
				context = new Context(scenario);
				Contexts.Add(context);
			}

			return context;
		}

		private void AddDefaultNamespaces()
		{
			Namespaces.AddNamespace("xbrli", "http://www.xbrl.org/2003/instance");
			Namespaces.AddNamespace("link", "http://www.xbrl.org/2003/linkbase");
			Namespaces.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
			Namespaces.AddNamespace("iso4217", "http://www.xbrl.org/2003/iso4217");
			Namespaces.AddNamespace("find", "http://www.eurofiling.info/xbrl/ext/filing-indicators");
			Namespaces.AddNamespace("xbrldi", "http://xbrl.org/2006/xbrldi");
		}

		public void RemoveUnusedObjects()
		{
			var used = Units.UsedUnits();
			this.Units.Clear();
			this.Units.AddRange(used);
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
			if(other != null)
			{
				if(this.SchemaReference.Equals(other.SchemaReference))
				{
					if(this.Units.Equals(other.Units))
					{
						if(this.FilingIndicators.Equals(other.FilingIndicators))
						{
							if(this.Contexts.Equals(other.Contexts))
							{
								if(this.Facts.Equals(other.Facts))
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

		#endregion

		private void RebuildNamespacesAfterRead()
		{
			var namespaces = new XmlNamespaceManager(new NameTable());
			foreach(var item in this.XmlSerializerNamespaces.ToArray())
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

		private void UpdateContextNamespaces(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
		{
			foreach(var context in contextsWithMembers)
			{
				foreach(var m in context.Scenario.ExplicitMembers)
				{
					if(string.IsNullOrEmpty(m.Dimension.Namespace))
					{
						m.Dimension = new XmlQualifiedName(m.Dimension.Name, this.DimensionNamespace);
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
						m.Dimension = new XmlQualifiedName(m.Dimension.Name, this.DimensionNamespace);
					}
					if(string.IsNullOrEmpty(m.Domain.Namespace))
					{
						m.Domain = new XmlQualifiedName(m.Domain.Name, this.TypedDomainNamespace);
					}
				}
			}
		}

		private void GetFactNamespace()
		{
			if(string.IsNullOrEmpty(this.FactNamespace))
			{
				var fact = this.Facts.FirstOrDefault();
				if(fact != null)
				{
					var ns = fact.Metric.Namespace;
					if(string.IsNullOrEmpty(ns))
					{
						var idx = fact.Metric.Name.IndexOf(':');
						if(idx != -1)
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
			if(contextWithTypedMembers != null)
			{
				var member = contextWithTypedMembers.Scenario.TypedMembers.First();
				var typedDomainNs = member.Domain.Namespace;
				if(string.IsNullOrEmpty(typedDomainNs))
				{
					typedDomainNs = namespaces.LookupNamespace(member.Domain.Name.Substring(0, member.Domain.Name.IndexOf(':')));
				}
				this.TypedDomainNamespace = typedDomainNs;
				var dimensionNs = member.Dimension.Namespace;
				if(string.IsNullOrEmpty(dimensionNs))
				{
					dimensionNs = namespaces.LookupNamespace(member.Dimension.Name.Substring(0, member.Dimension.Name.IndexOf(':')));
				}
				this.DimensionNamespace = dimensionNs;
			}
		}

		private void DimensionFromExplicitMembers(IXmlNamespaceResolver namespaces, IEnumerable<Context> contextsWithMembers)
		{
			if(string.IsNullOrEmpty(this.DimensionNamespace))
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
					this.DimensionNamespace = dimensionNs;
				}
			}
		}

		private static XmlSerializer Serializer = new XmlSerializer(typeof(Instance));

		public static Instance FromFile(string path)
		{
			Instance xbrl = null;

			using(var inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				xbrl = (Instance)Serializer.Deserialize(inputStream);
				xbrl.RebuildNamespacesAfterRead();
			}
			return xbrl;
		}

		public void ToFile(string path)
		{
			var ns = this.Namespaces.ToXmlSerializerNamespaces();

			var settings = new XmlWriterSettings {
				Indent = true,
				NamespaceHandling = NamespaceHandling.OmitDuplicates,
				Encoding = UTF8Encoding.UTF8
			};
			using(var writer = XmlWriter.Create(path, settings))
			{
				if(!string.IsNullOrEmpty(this.TaxonomyVersion))
				{
					writer.WriteProcessingInstruction("taxonomy-version", this.TaxonomyVersion);
				}

				Serializer.Serialize(writer, this, ns);
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

		public Fact AddFact(Context context, string metric, string unit, string decimals, string value)
		{
			return this.Facts.Add(context, metric, unit, decimals, value);
		}

		public Fact AddFact(Scenario scenario, string metric, string unit, string decimals, string value)
		{
			if(scenario == null)
			{
				throw new ArgumentNullException("scenario");
			}
			scenario.Instance = this;
			return this.Facts.Add(scenario, metric, unit, decimals, value);
		}
	}
}