namespace Diwen.Xbrl
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System;
	using System.Linq;

	public class UnitCollection : KeyedCollection<string, Unit>, IEquatable<IList<Unit>>
	{
		private Instance Instance;

		public UnitCollection()
		{

		}

		public UnitCollection(Instance instance)
			: this()
		{
			this.Instance = instance;
		}

		public void Add(string id, string measure)
		{
			base.Add(new Unit(id, measure));
		}

		public UnitCollection UsedUnits()
		{
			var result = new UnitCollection();
			foreach(var unit in this)
			{
				var fact = this.Instance.Facts.FirstOrDefault(f => f.Unit == unit);
				if(fact != null)
				{
					result.Add(unit);
				}
			}

			return result;
		}

		protected override string GetKeyForItem(Unit item)
		{
			string key = null;
			if(item != null)
			{
				key = item.Id;
			}
			return key;
		}


		#region IEquatable implementation

		public bool Equals(IList<Unit> other)
		{
			return this.ContentCompare(other);
		}

		#endregion
	}
}