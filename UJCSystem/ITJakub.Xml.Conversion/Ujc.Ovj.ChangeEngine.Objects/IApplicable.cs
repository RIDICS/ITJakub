using System.Collections.Generic;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    public interface IApplicable
    {
        /// <summary>
        /// If change defined by <see cref="ChangeBase.Pattern"/> is applicable on input string.
        /// If <see cref="ChangeBase.Pattern"/> is not found, 
        /// </summary>
        /// <param name="input">Strin in which <see cref="ChangeBase.Pattern"/> is searched.</param>
        /// <returns>Returns <value>true</value> if <see cref="ChangeBase.Pattern"/> in input string is found.</returns>
        bool IsApplicable(string input);

        /// <summary>
        /// If change defined by <see cref="ChangeBase.Pattern"/> is applicable on input string.
        /// If <see cref="ChangeBase.Pattern"/> is not found, 
        /// </summary>
        /// <param name="input">Strin in which <see cref="ChangeBase.Pattern"/> is searched.</param>
        /// <param name="exceptions">Additional exceptions applied to the input.</param>
        /// <returns>Returns <value>true</value> if <see cref="ChangeBase.Pattern"/> in input string is found.</returns>
        bool IsApplicable(string input, IList<IChange> exceptions);
    }
}