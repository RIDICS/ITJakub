using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    [XmlRoot("expression")]
    [XmlType("expression")]
    [DebuggerDisplay("{Pattern} [{Format}]")]

    public class Expression : IExpression
    {
        private Regex _regex;
        private string _pattern;
        private ChangeFormat _format;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Expression()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Expression(string pattern, ChangeFormat format)
        {
            Pattern = pattern;
            Format = format;
        }

        /// <summary>
        /// Pattern which is searched in input text.
        /// Interpretation of pattern is based on its format.
        /// </summary>
        [XmlAttribute("pattern")]
        public string Pattern
        {
            get { return _pattern; }
            set
            {
                _pattern = value;
                SetRegex();
            }
        }

        private void SetRegex()
        {
            if (_pattern == null) _regex = null;
            switch (_format)
            {
                case ChangeFormat.String:
                    break;
                case ChangeFormat.RegularExpression:
                    _regex = new Regex(_pattern);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal Regex Regex { get { return _regex; } }

        /// <summary>
        /// Format of <see cref="Expression"/> and <see cref="Replace"/>. How is it interpreted during applying change.
        /// </summary>
        [XmlAttribute("format")]
        public ChangeFormat Format
        {
            get { return _format; }
            set
            {
                _format = value;
                SetRegex();
            }
        }

        /// <summary>
        /// If change defined by <see cref="Expression"/> is applicable on input string.
        /// If <see cref="Expression"/> is not found, 
        /// </summary>
        /// <param name="input">Strin in which <see cref="Expression"/> is searched.</param>
        /// <returns>Returns <value>true</value> if <see cref="Expression"/> in input string is found.</returns>
        public virtual bool IsApplicable(string input)
        {
            switch (Format)
            {
                case ChangeFormat.String:
                    return input.Contains(Pattern);
                case ChangeFormat.RegularExpression:
                    return _regex.IsMatch(input);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// If change defined by <see cref="ChangeBase.Pattern"/> is applicable on input string.
        /// If <see cref="ChangeBase.Pattern"/> is not found, 
        /// </summary>
        /// <param name="input">Strin in which <see cref="ChangeBase.Pattern"/> is searched.</param>
        /// <param name="exceptions">Additional exceptions applied to the input.</param>
        /// <returns>Returns <value>true</value> if <see cref="ChangeBase.Pattern"/> in input string is found.</returns>
        public bool IsApplicable(string input, IList<IChange> exceptions)
        {
            if (!IsApplicable(input)) return false;
            IList<IOccurrence> expressionOccurrences = Occurrences(input);



            int numberOfOccurence = NumberOfOccurences(input);
            int numberOfExpceptions = 0;
            foreach (IChange change in exceptions)
            {
                if(IsSubset(this, change))
                    numberOfExpceptions += change.NumberOfOccurences(input);
            }
            return (numberOfOccurence > numberOfExpceptions);
        }

        private static bool IsSubset(IExpression one, IExpression two)
        {
            if (one.IsApplicable(two.Pattern) || two.IsApplicable(one.Pattern)) return true;
            return false;
        }

        /// <summary>
        /// Number of occurences of the pattern in input text.
        /// </summary>
        /// <param name="input">Input text in which the pattern is searched.</param>
        /// <returns>Number of occurneces of the pattern.</returns>
        public virtual int NumberOfOccurences(string input)
        {
            int result = 0;

            if (input != null)

                switch (Format)
                {
                    case ChangeFormat.String:
                        //result = input.Split(new[] { Pattern }, StringSplitOptions.None).Length - 1;
                        result = CountStringOccurrences(input, Pattern);
                        break;
                    case ChangeFormat.RegularExpression:
                        result = _regex.Matches(input).Count;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            return result;
        }

        /// <summary>
        /// Pointers to start indexes in the input, where patterns starts.
        /// </summary>
        /// <param name="input">Input text in which the pattern is searched.</param>
        /// <returns>List of integers. If there is no pattern match, empty list is returned.</returns>
        // při porovnánání výjimky se pak zjišťuje, jestli výskyt je mezi těmito krajními body
        public IList<IOccurrence> Occurrences(string input)
        {
            IList<IOccurrence> occurences = new Occurrences();
            
            switch (Format)
            {
                case ChangeFormat.String:
                    occurences = StringOccurrences(input, Pattern);
                    break;
                case ChangeFormat.RegularExpression:
                    MatchCollection matches = _regex.Matches(input);
                    foreach (Match match in matches)
                    {
                        occurences.Add(new Occurrence(match.Index, match.Index + match.Length));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return occurences;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the pattern
        /// in the input. The search starts at a specified character position.
        /// </summary>
        /// <param name="input">Input text in which the pattern is searched.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is startIndex.</returns>
        public int IndexOf(string input, int startIndex)
        {
            if(input == null) throw new ArgumentNullException("input");
            if (startIndex < 0 || startIndex >= input.Length) throw new ArgumentOutOfRangeException("startIndex");
            int result = -1;
            if (input.Length == 0) return startIndex;

            switch (Format)
            {
                case ChangeFormat.String:
                    result = input.IndexOf(Pattern, startIndex, StringComparison.CurrentCulture);
                    break;
                case ChangeFormat.RegularExpression:
                    result = Regex.Match(input, startIndex).Index;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        //http://www.dotnetperls.com/string-occurrence
        /// <summary>
        /// Count occurrences of strings.
        /// </summary>
        private static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        private static List<int> StringOccurrencePointers(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            List<int> pointers = new List<int>();
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                pointers.Add(i);
                i += pattern.Length;
            }
            return pointers;
        }

        private static IList<IOccurrence> StringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            IList<IOccurrence> occurrences = new Occurrences();

            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                occurrences.Add(new Occurrence(i, i+ pattern.Length));
                i += pattern.Length;
            }
            return occurrences;
        }

    }
}