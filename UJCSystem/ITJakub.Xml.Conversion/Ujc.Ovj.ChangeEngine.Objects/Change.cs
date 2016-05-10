using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects


{
    /// <summary>
    /// Class used for applying changes to input text.
    /// </summary>
    [XmlRoot("change")]
    [XmlType("change")]
    [DebuggerDisplay("{Order}. {Pattern} > {Replace} [{Format}; {Options}]")]
    public class Change : ChangeBase, IEquatable<Change>
    {

        public Change() : base(null, null, ChangeFormat.String)
        {
            
        }
        public Change(string pattern, string replace, ChangeFormat format) : base(pattern, replace, format)
        {
        }

        public Change(string pattern, string replace, ChangeFormat format, ChangeOptions options) : base(pattern, replace, format, options)
        {
        }

        public Change(int order, string pattern, string replace, ChangeFormat format, ChangeOptions options) : base(pattern, replace, format, options)
        {
            Order = order;
        }

        public Change(int order, string pattern, string replace, ChangeFormat format) : base(pattern, replace, format)
        {
            Order = order;
        }

        /// <remarks>http://stackoverflow.com/questions/5818513/xml-serialization-hide-null-values</remarks>
        public bool ShouldSerializeOptions()
        {
            return Options != null;
        }



        /// <summary>
        /// Order in wchich the change is applied
        /// </summary>
        [XmlAttribute("order")]
        public int Order { get; set; }

        /// <summary>
        /// If application of this change is disabled
        /// </summary>
        [XmlAttribute("disabled")]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Počet použití změny ve vstupních tokenech
        /// </summary>
        [XmlAttribute("exploitation")]
        public int Exploitation { get; set; }
        //TODO Zahrnout do IEquatable?

        public AppliedChange GetAppliedChange(string input)
        {
            if (!IsApplicable(input)) return null;
            return new AppliedChange(input, Apply(input), this);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Change other)
        {
            if (other == null) return false;
            return Equals(other.Pattern, Pattern) && Equals(other.Replace, Replace)
                   && other.Format == Format && Equals(other.Options, Options)
                   && other.Order == Order && other.IsDisabled == IsDisabled;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Change)) return false;
            return Equals((Change)obj);
        }

        public override int GetHashCode()
        {
            const int prime = 397;
            int result = (int)Format;
            result = (result * prime) ^ (Pattern != null ? Pattern.GetHashCode() : 0);
            result = (result * prime) ^ (Replace != null ? Replace.GetHashCode() : 0);
            result = (result * prime) ^ (Options != null ? Options.GetHashCode() : 0);
            result = (result * prime) ^ (Order.GetHashCode());
            result = (result * prime) ^ (IsDisabled.GetHashCode());
            return result;
        }

        public static bool operator == (Change left, Change right)
        {
            // If we used == to check for null instead of Object.ReferenceEquals(), we'd
            // get a StackOverflowException. Can you figure out why?
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return true;
            if (ReferenceEquals(left, null)) return false;
            return left.Equals(right);
        }

        public static bool operator != (Change left, Change right)
        {
            // Since we've already defined ==, we can just invert it for !=.
            return !(left == right);
        }

    }
}
