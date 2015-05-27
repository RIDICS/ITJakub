﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Campari.Software.Reflection
{
    #region struct KeyValueTriplet
    /// <summary>
    /// Defines a key/numeric key/value triplet that can be set or retrieved.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TNumericKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct KeyValueTriplet<TKey, TNumericKey, TValue>
    {
        #region class-wide fields
        private TKey key;
        private TValue value;
        private TNumericKey numericKey;
        #endregion

        #region public properties and methods

        #region properties

        #region Key
        /// <summary>
        /// Gets the key in the key/numeric key/value triplet.
        /// </summary>
        /// <value>A <typeparamref name="TKey"/> that is the key of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>.</value>
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }
        #endregion

        #region NumericKey
        /// <summary>
        /// Gets the numeric representation of the <see cref="Key"/> in the key/numeric key/value triplet.
        /// </summary>
        /// <value>A <typeparamref name="TNumericKey"/> that is the numeric key of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>.</value>
        public TNumericKey NumericKey
        {
            get
            {
                return this.numericKey;
            }
        }
        #endregion

        #region Value
        /// <summary>
        /// Gets the value in the key/numeric key/value triplet.
        /// </summary>
        /// <value>A <typeparamref name="TValue"/> that is the value of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>.</value>
        public TValue Value
        {
            get
            {
                return this.value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region constructor
        /// <summary>
        /// Inititalizes a new instance of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>
        /// structure with the specified key, numeric key, and value.
        /// </summary>
        /// <param name="key">The object defined in each key/numeric key/value triplet.</param>
        /// <param name="numericKey">The numeric representation of each <paramref name="key"/> 
        /// defined in each key/numeric key/value triplet.</param>
        /// <param name="value">The definition associate with <paramref name="key"/>.</param>
        public KeyValueTriplet(TKey key, TNumericKey numericKey, TValue value)
        {
            this.key = key;
            this.value = value;
            this.numericKey = numericKey;
        }
        #endregion

        #region ToString
        /// <summary>
        /// Returns a string representation of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>,
        /// using the string representations of the key, numeric key, and value.
        /// </summary>
        /// <returns>A string representation of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>,
        /// which includes the string representations of the key, numeric key, and value.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            if (this.Key != null)
            {
                builder.Append(this.Key.ToString());
            }
            builder.Append(", ");
            if (this.NumericKey != null)
            {
                builder.Append(this.NumericKey.ToString());
            }
            builder.Append(", ");
            if (this.Value != null)
            {
                builder.Append(this.Value.ToString());
            }
            builder.Append(']');
            return builder.ToString();
        }
        #endregion

        #endregion

        #endregion
    }
    #endregion
}
