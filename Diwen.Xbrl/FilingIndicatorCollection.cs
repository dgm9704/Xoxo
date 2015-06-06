namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public class FilingIndicatorCollection : Collection<FilingIndicator>, IEquatable<IList<FilingIndicator>>
	{

		//		public FilingIndicatorCollection(Instance instance)
		//		{
		//		}

		public FilingIndicator Add(Context context, string value)
		{
			var filingIndicator = new FilingIndicator(context, value);
			base.Add(filingIndicator);
			return filingIndicator;
		}

		#region IEquatable implementation

		public bool Equals(IList<FilingIndicator> other)
		{
			return this.SmartCompare(other);
		}

		#endregion
	}
}