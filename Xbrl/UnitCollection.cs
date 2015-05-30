using System.Linq;

namespace Xoxo
{
	using System.Xml.Schema;
	using System.Xml;
	using System.Collections.Generic;
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	public class UnitCollection : Collection<Unit>, IEquatable<UnitCollection>
	{
		private XbrlInstance Instance;

		public UnitCollection()
		{
			this.Add("uEUR", "iso4217:EUR");
			this.Add("uPURE", "xbrli:pure"); 
		}

		public UnitCollection(XbrlInstance instance) : this()
		{
			this.Instance = instance;
		}

		public void Add(string id, string measure)
		{
			base.Add(new Unit(id, measure));
		}

		public Collection<Unit> UsedUnits()
		{
			var result = new Collection<Unit>();
			foreach(var unit in this)
			{
				var fact = this.Instance.Facts.FirstOrDefault(f => f.Unit == unit.Id);
				if(fact != null)
				{
					result.Add(unit);
				}
			}

			return result;
		}

		#region IEquatable implementation

		public bool Equals(UnitCollection other)
		{
			var result = true;

			if(this.Count != other.Count)
			{
				result = false;
			}
			else
			{
				for(int i = 0; i < this.Count; i++)
				{
					if(!this[i].Equals(other[i]))
					{
						result = false;
						break;
					}
				}
			}

			return result;
		}

		#endregion
	}
}