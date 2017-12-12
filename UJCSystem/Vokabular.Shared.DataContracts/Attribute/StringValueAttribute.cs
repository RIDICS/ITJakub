using System;

namespace Vokabular.Shared.DataContracts.Attribute
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : System.Attribute
    {
        #region Properties

        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            StringValue = value;
        }

        #endregion
    }
}