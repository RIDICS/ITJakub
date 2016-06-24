using System;
using System.Diagnostics;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    [DebuggerDisplay("Start: {StartIndex}, End: {EndIndex}")]
    public class Occurrence : IOccurrence, IEquatable<IOccurrence>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Occurrence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Occurrence(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        #region Implementation of IOccurrence

        /// <summary>
        /// Start index; index (in the string) wehre occurence starts.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// End index; index (in the string) where occurrence ends.
        /// </summary>
        public int EndIndex { get; set; }

        #endregion

        #region Equality members

        protected bool Equals(Occurrence other)
        {
            //return StartIndex == other.StartIndex && EndIndex == other.EndIndex;
            return Equals((IOccurrence) other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Occurrence) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (StartIndex*397) ^ EndIndex;
            }
        }

        #endregion

        #region Implementation of IEquatable<IOccurrence>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IOccurrence other)
        {
            return StartIndex == other.StartIndex && EndIndex == other.EndIndex;
        }

        #endregion
    }
}