namespace Xoxo
{
	using System;
	using System.Collections.ObjectModel;
	using System.Globalization;

	public class ContextCollection : KeyedCollection<string, Context>, IEquatable<ContextCollection>
	{
		private IFormatProvider ic = CultureInfo.InvariantCulture;

		private XbrlInstance Instance;

		public string IdFormat { get; set; }

		public ContextCollection()
		{
			this.IdFormat = "A{0}";
		}

		public ContextCollection(XbrlInstance instance) : this()
		{
			this.Instance = instance;
		}

		public new string Add(Context context)
		{
			context.Entity = Instance.ContextEntity;
			context.Period = Instance.ContextPeriod;

			var exists = false;
			foreach(var oldContext in this)
			{
				if(context.Equals(oldContext))
				{
					exists = true;
					context = oldContext;
					break;
				}
			}

			if(!exists)
			{
				context.Id = NextId();
				base.Add(context);
			}

			return context.Id;
		}

		public string NextId()
		{
			var counter = this.Count;
			string id;
			do
			{
				id = string.Format(ic, this.IdFormat, ++counter);
			}
			while(this.Contains(id));

			return id;
		}

		public Context Add()
		{
			var id = NextId();
			var context = new Context(id);
			this.Add(context);
			return context;
		}

		protected override string GetKeyForItem(Context item)
		{
			return item.Id;
		}

		#region IEquatable implementation

		public bool Equals(ContextCollection other)
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