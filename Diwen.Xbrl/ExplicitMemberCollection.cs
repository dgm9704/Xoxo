namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Xml;
	using System.Xml.Serialization;

	public class ExplicitMemberCollection : Collection<ExplicitMember>, IEquatable<IList<ExplicitMember>>
	{
		private Instance instanceField;
		private IFormatProvider ic = CultureInfo.InvariantCulture;

		[XmlIgnore]
		public Instance Instance
		{
			get { return instanceField; }
			set
			{
				this.instanceField = value;
				foreach(var item in this)
				{
					if(item.Dimension.Namespace != Instance.DimensionNamespace)
					{
						var dimensionNs = instanceField.DimensionNamespace;
						item.Dimension = new XmlQualifiedName(item.Dimension.LocalName(), dimensionNs);
					}

					if(string.IsNullOrEmpty(item.Value.Namespace))
					{
						string valNs = this.Instance.Namespaces.LookupNamespace(item.Value.Prefix());

						if(!string.IsNullOrEmpty(valNs))
						{
							if(item.Value.Namespace != valNs)
							{
								item.Value = new XmlQualifiedName(item.Value.LocalName(), valNs);
							}
						}
						else
						if(this.Instance.CheckExplicitMemberDomainExists)
						{
							throw new InvalidOperationException(string.Format(ic, "No namespace declared for domain '{0}'", item.Value.Prefix()));
						}
					}
				}
			}
		}

		public ExplicitMemberCollection()
		{
		}

		public ExplicitMemberCollection(Instance instance)
			: this()
		{
			this.Instance = instance;
		}


		public ExplicitMember Add(string dimension, string value)
		{
			if(string.IsNullOrEmpty(dimension))
			{
				throw new ArgumentOutOfRangeException("dimension");
			}

			if(string.IsNullOrEmpty(value))
			{
				throw new ArgumentOutOfRangeException("value");
			}

			XmlQualifiedName dim;
			XmlQualifiedName val;

			if(this.Instance != null)
			{
				string dimNs = this.Instance.DimensionNamespace;
				string valPrefix = value.Substring(0, value.IndexOf(':'));
				string valNs = this.Instance.Namespaces.LookupNamespace(valPrefix);
				if(this.Instance.CheckExplicitMemberDomainExists)
				{
					if(string.IsNullOrEmpty(valNs))
					{
						throw new InvalidOperationException(string.Format(ic, "No namespace declared for domain '{0}'", valPrefix));
					}
				}

				dim = new XmlQualifiedName(dimension, dimNs);
				val = new XmlQualifiedName(value, valNs);
			}
			else
			{
				dim = new XmlQualifiedName(dimension);
				val = new XmlQualifiedName(value);
			}

			var explicitMember = new ExplicitMember(dim, val);
			base.Add(explicitMember);
			return explicitMember;
		}

		#region IEquatable implementation

		public bool Equals(IList<ExplicitMember> other)
		{
			return this.ContentCompare(other);
		}

		#endregion
	}
}