using System;
using System.Xml;

namespace Xoxo
{
	public static class XblrInstanceExtensions
	{
		public static Context NewContext(this XbrlInstance instance)
		{
			return instance.Contexts.Add();
		}

		public static FilingIndicator AddFilingIndicator(this XbrlInstance instance, string value)
		{
			var context = instance.GetContext(null);
			return AddFilingIndicator(instance, context, value);
		}

		public static FilingIndicator AddFilingIndicator(this XbrlInstance instance, Context context, string value)
		{
			return instance.FilingIndicators.Add(context, value);
		}

		public static Fact AddFact(this XbrlInstance instance, Context context, string metric, string unit, string decimals, string value)
		{
			return instance.Facts.Add(context, metric, unit, decimals, value);
		}

		public static Fact AddFact(this XbrlInstance instance, Scenario scenario, string metric, string unit, string decimals, string value)
		{
			scenario.Instance = instance;

			if(!metric.StartsWith(instance.FactPrefix))
			{
				metric = instance.FactPrefix + ":" + metric;
			}

			return instance.Facts.Add(scenario, metric, unit, decimals, value);
		}
	}
}

