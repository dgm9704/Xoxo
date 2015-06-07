namespace Diwen.Xbrl
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot(ElementName = "explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
	public class ExplicitMember : IEquatable<ExplicitMember>, IComparable<ExplicitMember>
	{
		[XmlAttribute("dimension", Namespace = "http://xbrl.org/2006/xbrldi")]
		public XmlQualifiedName Dimension { get; set; }

		[XmlText]
		public XmlQualifiedName Value { get; set; }

		public ExplicitMember()
		{
		}

		public ExplicitMember(XmlQualifiedName dimension, XmlQualifiedName value)
			: this()
		{
			this.Dimension = dimension;
			this.Value = value;
		}

		public override int GetHashCode()
		{
			return this.Dimension.GetHashCode()
			^ this.Value.GetHashCode();
		}

		#region IEquatable implementation

		public bool Equals(ExplicitMember other)
		{
			var result = false;
			if(other != null)
			{
				if(this.Dimension == other.Dimension)
				{
					if(this.Value == other.Value)
					{
						result = true;
					}
				}

			}

			return result;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if(obj is ExplicitMember)
			{
				return this.Equals((obj as ExplicitMember));
			}
			else
			{
				return base.Equals(obj);
			}
		}

		#region IComparable implementation

		public int CompareTo(ExplicitMember other)
		{
			int result = 0;
			if(other == null)
			{
				result = 1;
			}
			else
			{
				result = string.Compare(this.Dimension.Name, other.Dimension.Name, StringComparison.OrdinalIgnoreCase);
				if(result == 0)
				{
					result = string.Compare(this.Value.Name, other.Value.Name, StringComparison.OrdinalIgnoreCase);
				}
			}

			return result;
		}

		#endregion

		public static bool operator ==(ExplicitMember left, ExplicitMember right)
		{
			var result = false;

			// If both are null, or both are same instance, return true.
			if(object.ReferenceEquals(left, right))
			{
				result = true;
			}
			// If one is null, but not both, return false.
			else if(((object)left == null) || ((object)right == null))
			{
				result = false;
			}
			else
			{
				// Return true if the fields match:
				result = left.Equals(right);
			}

			return result;
		}

		public static bool operator !=(ExplicitMember left, ExplicitMember right)
		{
			var result = false;

			// If one is null, but not both, return true.
			if(((object)left == null) || ((object)right == null))
			{
				result = true;
			}
			else
			{
				result = !left.Equals(right);
			}

			return result;
		}

		public static bool operator >(ExplicitMember left, ExplicitMember right)
		{
			var result = false;

			// If both are null, or both are same instance, return false.
			if(object.ReferenceEquals(left, right))
			{
				result = false;
			}
			else
			{
				result = left != null && left.CompareTo(right) > 0;
			}

			return result;
		}

		public static bool operator <(ExplicitMember left, ExplicitMember right)
		{
			return right > left;
		}

		public int Compare(ExplicitMember other)
		{
			return this.CompareTo(other);
		}
	}
}