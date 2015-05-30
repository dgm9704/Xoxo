namespace Xoxo
{
	public static class ScenarioExtensions
	{
		public static ExplicitMember AddExplicitMember(this Scenario scenario, string dimension, string value)
		{
			return scenario.ExplicitMembers.Add(dimension, value);
		}

		public static TypedMember AddTypedMember(this Scenario scenario, string dimension, string domain, string value)
		{
			return scenario.TypedMembers.Add(dimension, domain, value);
		}
	}
}