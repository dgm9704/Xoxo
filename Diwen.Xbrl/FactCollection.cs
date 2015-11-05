namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.Linq;

	public class FactCollection : Collection<Fact>, IEquatable<IList<Fact>>
	{
		private Instance Instance;
		private static IFormatProvider ic = CultureInfo.InvariantCulture;

		public FactCollection(Instance instance)
		{
			this.Instance = instance;
		}

		public Fact Add(Context context, string metric, string unitRef, string decimals, string value)
		{
			var ns = this.Instance.FactNamespace;
			var prefix = this.Instance.Namespaces.LookupPrefix(ns);

			Unit unit = null;
			if(!string.IsNullOrEmpty(unitRef))
			{
				if(!this.Instance.Units.Contains(unitRef))
				{
					throw new InvalidOperationException(string.Format(ic, "Referenced unit '{0}' does not exist", unitRef));
				}
				unit = this.Instance.Units[unitRef];
			}

			var fact = new Fact(context, metric, unit, decimals, value, ns, prefix);
			base.Add(fact);
			return fact;
		}

		public Fact Add(Scenario scenario, string metric, string unit, string decimals, string value)
		{
			var context = Instance.GetContext(scenario);
			return Add(context, metric, unit, decimals, value);
		}

		#region IEquatable implementation

		public bool Equals(IList<Fact> other)
		{
			return this.ContentCompare(other);
		}

		#endregion
	}
}