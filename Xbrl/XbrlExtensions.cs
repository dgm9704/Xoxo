namespace Xoxo
{
	using System;
	using System.Xml;

	public static class XbrlExtensions
	{
		//		public static Context NewContext(this Xbrl instance)
		//		{
		//			return instance.Contexts.Add();
		//		}

		public static FilingIndicator AddFilingIndicator(this Xbrl instance, string value)
		{
			var context = instance.GetContext(null);
			return AddFilingIndicator(instance, context, value);
		}

		public static FilingIndicator AddFilingIndicator(this Xbrl instance, Context context, string value)
		{
			return instance.FilingIndicators.Add(context, value);
		}

		public static Fact AddFact(this Xbrl instance, Context context, string metric, string unit, string decimals, string value)
		{
			return instance.Facts.Add(context, metric, unit, decimals, value);
		}

		public static Fact AddFact(this Xbrl instance, Scenario scenario, string metric, string unit, string decimals, string value)
		{
			scenario.Instance = instance;

			return instance.Facts.Add(scenario, metric, unit, decimals, value);
		}
	}
}

