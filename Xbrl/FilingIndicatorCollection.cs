namespace Xoxo
{
	using System.Xml.Schema;
	using System.Xml;
	using System.Collections.Generic;
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	public class FilingIndicatorCollection : Collection<FilingIndicator> , IEquatable<FilingIndicatorCollection>
	{
		private XbrlInstance Instance;

		public FilingIndicatorCollection(XbrlInstance instance)
		{
			this.Instance = instance;
		}

		public void Add(string contextId, string value)
		{
			base.Add(new FilingIndicator(contextId, value));
		}

		#region IEquatable implementation

		public bool Equals(FilingIndicatorCollection other)
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