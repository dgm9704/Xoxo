//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2016 John Nordberg
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
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;

	public class Fact : IEquatable<Fact>
	{
		static XmlDocument doc = new XmlDocument();

		[XmlIgnore]
		public Unit Unit { get; set; }

		[XmlIgnore]
		public string Decimals { get; set; }

		[XmlIgnore]
		public Context Context { get; set; }

		[XmlIgnore]
		public XmlQualifiedName Metric { get; set; }

		[XmlIgnore]
		public string Value { get; set; }

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
				if (value != null)
				{
					foreach (var element in value)
					{
						Facts.Add(FromXmlElement(element));
					}
				}
			}
		}

		internal string ContextRef;
		internal string UnitRef;

		public Fact()
		{
			Facts = new FactCollection(null);
		}

		public override string ToString()
		{
			var metric = Metric != null ? Metric.LocalName() : string.Empty;
			var measure = Unit != null ? Unit.Measure : string.Empty;
			return $"Metric={metric}, Value={Value}, Unit={measure}, Decimals={Decimals}, Context={ContextRef}";
		}

		public Fact(Context context, string metric, Unit unit, string decimals, string value, string namespaceUri, string prefix)
			: this(context, metric, unit, decimals, value, new Uri(namespaceUri), prefix)
		{
		}

		public Fact(Context context, string metric, Unit unit, string decimals, string value, Uri namespaceUri, string prefix)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (namespaceUri == null)
			{
				throw new ArgumentNullException(nameof(namespaceUri));
			}

			Facts = new FactCollection(null);
			Metric = new XmlQualifiedName($"{prefix}:{metric}", namespaceUri.ToString());
			Unit = unit;
			Decimals = decimals;
			Context = context;
			Value = value;
		}

		public Fact AddFact(Context context, string metric, string unitRef, string decimals, string value)
		{
			return Facts.Add(context, metric, unitRef, decimals, value);
		}

		public Fact AddFact(Scenario scenario, string metric, string unitRef, string decimals, string value)
		{
			if (scenario != null)
			{
				Facts.Instance = scenario.Instance;

				if (scenario.ExplicitMembers.Count == 0 && scenario.TypedMembers.Count == 0)
				{
					scenario = null;
				}
			}
			return Facts.Add(scenario, metric, unitRef, decimals, value);
		}

		public Fact AddFact(Segment segment, string metric, string unitRef, string decimals, string value)
		{
			if (segment != null)
			{
				Facts.Instance = segment.Instance;

				if (segment.ExplicitMembers.Count == 0 && segment.TypedMembers.Count == 0)
				{
					segment = null;
				}
			}

			return Facts.Add(segment, metric, unitRef, decimals, value);
		}

		internal XmlElement ToXmlElement()
		{
			var element = doc.CreateElement(Metric.Name, Metric.Namespace);

			if (Facts.Any())
			{
				Facts.
					 ToList().
					 ForEach(f => element.AppendChild(f.ToXmlElement()));
			}
			else
			{
				if (Context != null)
				{
					element.SetAttribute("contextRef", Context.Id);
				}

				if (Unit != null)
				{
					element.SetAttribute("unitRef", Unit.Id);
				}
				if (!string.IsNullOrEmpty(Decimals))
				{
					element.SetAttribute("decimals", Decimals);
				}

				element.InnerText = Value;
			}
			return element;
		}

		internal static Fact FromXmlElement(XmlElement element)
		{
			var fact = new Fact();
			fact.Metric = new XmlQualifiedName(element.Name, element.NamespaceURI);

			if (element.InnerXml == element.InnerText)
			{
				fact.UnitRef = element.GetAttribute("unitRef");
				fact.Decimals = element.GetAttribute("decimals");
				fact.ContextRef = element.GetAttribute("contextRef");
				fact.Value = element.InnerText;
			}
			else
			{
				fact.UnitRef = "";
				fact.Decimals = "";
				fact.ContextRef = "";
				fact.Value = "";
			}
			return fact;
		}

		public override bool Equals(object obj)
		{
			var result = false;
			var other = obj as Fact;
			if (other != null && Equals(other))
			{
				result |= Facts.Equals(other.Facts);
			}
			if (!result)
			{
				Console.WriteLine("Schema references different");
			}
			return result;
		}

		public override int GetHashCode()
		{
			return Metric.GetHashCode();
		}

		#region IEquatable implementation

		public bool Equals(Fact other)
		{
			var result = other != null
						 && Metric.Equals(other.Metric)
						 && Value.Equals(other.Value, StringComparison.Ordinal)
						 && Decimals.Equals(other.Decimals, StringComparison.Ordinal);
			if (result)
			{
				result = Unit == null
					? other.Unit == null
					: Unit.Equals(other.Unit);
			}
			return result;
		}

		#endregion
	}
}