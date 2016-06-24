using System.Collections.Generic;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    public interface IChange : IExpression
    {
        /// <summary>
        /// How is pattern replaced in input text, if it is founded.
        /// </summary>
        string Replace { get; set; }

        /// <summary>
        /// Option applied during change.
        /// </summary>
        ChangeOptions Options { get; set; }

        /// <summary>
        /// Applies change defined by <see cref="ChangeBase.Replace"/> to input string.
        /// </summary>
        /// <param name="input">String on which change is applied.</param>
        /// <returns>Returns new version of string if <see cref="ChangeBase.Pattern"/> in <see cref="input"/> string is found.</returns>
        /// <remarks>Applied changes depends on <see cref="ChangeBase.Options"/> defined for change.</remarks>
        string Apply(string input);

        /// <summary>
        /// Applies change defined by <see cref="ChangeBase.Replace"/> to input string excluding occurences in exceptions.
        /// </summary>
        /// <param name="input">String on which change is applied.</param>
        /// <param name="exceptions">Exceptions where pattern cannot be applied.</param>
        /// <returns>Returns new version of string if <see cref="ChangeBase.Pattern"/> in <see cref="input"/> string is found.</returns>
        /// <remarks>Applied changes depends on <see cref="ChangeBase.Options"/> defined for change.</remarks>
        string Apply(string input, IList<Change> exceptions);

    }
}