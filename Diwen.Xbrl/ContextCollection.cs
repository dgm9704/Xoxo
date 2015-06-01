namespace Diwen.Xbrl
{
    using System.Collections.ObjectModel;
    using System;
    using System.Globalization;

    public class ContextCollection : KeyedCollection<string, Context>, IEquatable<ContextCollection>
    {
        private IFormatProvider ic = CultureInfo.InvariantCulture;

        private Xbrl Instance;

        public string IdFormat { get; set; }

        public ContextCollection()
        {
            this.IdFormat = "A{0}";
        }

        public ContextCollection(Xbrl instance)
            : this()
        {
            this.Instance = instance;
        }

        public new Context Add(Context context)
        {
            if (context.Entity == null)
            {
                context.Entity = Instance.Entity;
            }

            if (context.Period == null)
            {
                context.Period = Instance.Period;
            }

            if (string.IsNullOrEmpty(context.Id))
            {
                var exists = false;
                foreach (var oldContext in this)
                {
                    if (context.Equals(oldContext))
                    {
                        exists = true;
                        context = oldContext;
                        break;
                    }
                }

                if (!exists)
                {
                    context.Id = NextId();
                    base.Add(context);
                }
            }
            else
            {
                base.Add(context);
            }

            return context;
        }

        public string NextId()
        {
            var counter = this.Count;
            string id;
            do
            {
                id = string.Format(ic, this.IdFormat, counter++);
            }
            while(this.Contains(id));

            return id;
        }

        protected override string GetKeyForItem(Context item)
        {
            return item.Id;
        }

        #region IEquatable implementation

        public bool Equals(ContextCollection other)
        { 
            var result = true;

            if (this.Count != other.Count)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (!this[i].Equals(other[i]))
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