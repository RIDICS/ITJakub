using System;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    /// <summary>
    /// Options applied during change of input.
    /// </summary>
    [XmlRoot("options")]
    [XmlType("options")]
    public class ChangeOptions : IEquatable<ChangeOptions>
    {
        public ChangeOptions()
        {
            ReplacementCount = null;
        }

        /// <summary>
        /// How many times pattern in input is replaced.
        /// </summary>
        /// <remarks>Defaul value is <value>null</value>.</remarks>
        /// <remarks>To change all occurences of pattern set value to <value>null</value> or <value>-1</value>.</remarks>
        [XmlElement(ElementName = "replacement-count", IsNullable = true)]
        public int? ReplacementCount { get; set; }

        /// <summary>
        /// List of exceptions in which pattern is not applicable.
        /// </summary>
        [XmlArray(ElementName = "exceptions", IsNullable = true)]
        [XmlArrayItem(ElementName = "exception")]
        public Expressions Exceptions { get; set; }
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ChangeOptions other)
        {
            if (other == null) return false;
            return other.ReplacementCount == ReplacementCount;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (!(obj is ChangeOptions)) return false;
            return Equals((ChangeOptions)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            //const int prime = 397;
            int result = (ReplacementCount.HasValue ? ReplacementCount.Value : 0);
            return result;
        }

        public static bool operator ==(ChangeOptions left, ChangeOptions right)
        {
            // If we used == to check for null instead of Object.ReferenceEquals(), we'd
            // get a StackOverflowException. Can you figure out why?
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return true;
            if (ReferenceEquals(left, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(ChangeOptions left, ChangeOptions right)
        {
            // Since we've already defined ==, we can just invert it for !=.
            return !(left == right);
        }
    }
}
