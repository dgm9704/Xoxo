namespace Xoxo
{
	using System;
	using System.Collections.ObjectModel;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	public class FactCollection : Collection<Fact> , IEquatable<FactCollection>
	{
		private XbrlInstance Instance;

		public FactCollection(XbrlInstance instance)
		{
			this.Instance = instance;
		}

		public void Add(string metric, string unit, string decimals, string context, string value)
		{
			base.Add(new Fact(metric, unit, decimals, context, value, Instance.FactNamespace));
		}

		#region IEquatable implementation

		public bool Equals(FactCollection other)
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