using System.Collections.Generic;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    public interface IExpression : IApplicable
    {
        /// <summary>
        /// Pattern which is searched in input text.
        /// Interpretation of pattern is based on its format.
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// Format of <see cref="Pattern"/>. How is it interpreted during pattern recognition.
        /// </summary>
        ChangeFormat Format { get; set; }

        /// <summary>
        /// Number of occurences of the pattern in input text.
        /// </summary>
        /// <param name="input">Input text in which the pattern is searched.</param>
        /// <returns>Number of occurneces of the pattern.</returns>
        int NumberOfOccurences(string input);

        /// <summary>
        /// Pointers to start indexes in the input, where patterns starts.
        /// </summary>
        /// <param name="input">Input text in which the pattern is searched.</param>
        /// <returns>List of integers. If there is no pattern match, empty list is returned.</returns>
        IList<IOccurrence> Occurrences(string input);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the pattern
        /// in the input. The search starts at a specified character position.
        /// </summary>
        /// <param name="input">Input text in which the pattern is searched.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is startIndex.</returns>
        int IndexOf(string input, int startIndex);

    }
}