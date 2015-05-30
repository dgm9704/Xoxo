using System;

namespace Xoxo
{
	public static class ContextExtensions
	{
		public static ExplicitMember AddExplicitMember(this Context context, string dimension, string value)
		{
			return context.Scenario.ExplicitMembers.Add(dimension, value);
		}

		public static TypedMember AddTypedMember(this Context context, string dimension, string domain, string value)
		{
			return context.Scenario.TypedMembers.Add(dimension, domain, value);
		}
	}
}

