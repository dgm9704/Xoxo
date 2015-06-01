namespace Diwen.Xbrl
{
    using System.Collections.ObjectModel;
    using System;
    using System.Linq;

    public class FactCollection : Collection<Fact> , IEquatable<FactCollection>
    {
        private Xbrl Instance;

        public FactCollection(Xbrl instance)
        {
            this.Instance = instance;
        }

        public Fact Add(Context context, string metric, string unit, string decimals, string value)
        {
            var ns = this.Instance.FactNamespace;
            var prefix = this.Instance.Namespaces.LookupPrefix(ns);

            if (this.Instance.CheckUnitExists)
            {
                if (!this.Instance.Units.Exists(u => u.Id == unit))
                {
                    throw new InvalidOperationException(string.Format("Referenced unit '{0}' does not exist", unit));
                }
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

        public bool Equals(FactCollection other)
        { 
            return this.SequenceEqual(other);
        }

        #endregion
    }

}