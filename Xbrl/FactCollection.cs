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

		public Fact Add(Context context, string metric, string unit, string decimals, string value)
		{
			if(!metric.StartsWith(Instance.FactPrefix))
			{
				metric = Instance.FactPrefix + ":" + metric;
			}
			var fact = new Fact(context, metric, unit, decimals, value, Instance.FactNamespace);
			base.Add(fact);
			return fact;
		}

		public Fact Add(Scenario scenario, string metric, string unit, string decimals, string value)
		{
			var context = Instance.GetContext(scenario);
			return Add(context, metric, unit, decimals, value);
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