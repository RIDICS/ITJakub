using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Texty.Evidence
{

    public class PrepisComparer<Prepis> : IComparer<Prepis>
    {
        private string PropertyName;

        /// <summary>
        /// Provides Comparison opreations.
        /// </summary>
        /// <param name="propertyName">The property to compare</param>
        public PrepisComparer(string propertyName)
        {
            PropertyName = propertyName;
            if (PropertyName == "DataceText")
            {
                PropertyName = "DataceDetaily";
            }
        }


        #region IComparer<Prepis> Members

        /// <summary>
        /// Compares 2 objects by their properties, given on the constructor
        /// </summary>
        /// <param name="x">First value to compare</param>
        /// <param name="y">Second value to compare</param>
        /// <returns></returns>
        public int Compare(Prepis x, Prepis y)
        {

            if (x != null && y == null)
                return 1;

            if (x == null && y != null)
                return -1;

            if (x == null && y == null)
                return 0;

            object a = x.GetType().GetProperty(PropertyName).GetValue(x, null);
            object b = y.GetType().GetProperty(PropertyName).GetValue(y, null);

            if (a != null && b == null)
                return 1;

            if (a == null && b != null)
                return -1;

            if (a == null && b == null)
                return 0;

            return ((IComparable)a).CompareTo(b);
        }

        #endregion
    }

}
