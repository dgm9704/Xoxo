//
//  This file is part of Diwen.xbrl.
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

					item.Instance = value;

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