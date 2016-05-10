using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    /// <summary>
    /// Class for registering change  applied to input text.
    /// </summary>
    
    [XmlRoot("appliedChange")]
    [DebuggerDisplay("{Input} > {Output} (Change: {Change.Pattern} > {Change.Replace} [{Change.Format}; {Change.Options}])")]
    public class AppliedChange : IEquatable<AppliedChange>
    {
        [XmlAttribute("input")]
        public string Input { get; set; }
        [XmlAttribute("output")]
        public string Output { get; set; }

        [XmlElement("change")]
        public Change Change { get; set; }

        public AppliedChange()
        {
        }

        public AppliedChange(string input, string output, Change change)
        {
            Input = input;
            Output = output;
            Change = change;
        }

        public bool Equals(AppliedChange other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Input, other.Input) && string.Equals(Output, other.Output) && Equals(Change, other.Change);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AppliedChange) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Input != null ? Input.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Output != null ? Output.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Change != null ? Change.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(AppliedChange left, AppliedChange right)
        {
            // If we used == to check for null instead of Object.ReferenceEquals(), we'd
            // get a StackOverflowException. Can you figure out why?
            if (Object.ReferenceEquals(left, null) && Object.ReferenceEquals(right, null)) return true;
            if (Object.ReferenceEquals(left, null)) return false;
            else return left.Equals(right);
        }

        public static bool operator !=(AppliedChange left, AppliedChange right)
        {
            // Since we've already defined ==, we can just invert it for !=.
            return !(left == right);
        }
 
    }
}
