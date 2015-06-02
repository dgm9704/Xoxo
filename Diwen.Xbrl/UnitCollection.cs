namespace Diwen.Xbrl
{
    using System.Collections.Generic;
    using System;
    using System.Linq;

    public class UnitCollection : List<Unit>, IEquatable<UnitCollection>
    {
        private Instance Instance;

        public UnitCollection()
        {

        }

        public UnitCollection(Instance instance)
            : this()
        {
            this.Instance = instance;
            this.Add("uEUR", "iso4217:EUR");
            this.Add("uPURE", "xbrli:pure"); 
        }

        public void Add(string id, string measure)
        {
            base.Add(new Unit(id, measure));
        }

        public UnitCollection UsedUnits()
        {
            var result = new UnitCollection();
            foreach (var unit in this)
            {
                var fact = this.Instance.Facts.FirstOrDefault(f => f.Unit == unit.Id);
                if (fact != null)
                {
                    result.Add(unit);
                }
            }

            return result;
        }

        #region IEquatable implementation

        public bool Equals(UnitCollection other)
        {
            var result = this.SequenceEqual(other);
            return result;
        }

        #endregion
    }
}