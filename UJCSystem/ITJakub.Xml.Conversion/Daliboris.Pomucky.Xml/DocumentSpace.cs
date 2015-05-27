using System.Collections.Generic;
using System.Diagnostics;

namespace Daliboris.Pomucky.Xml
{
	public class DocumentSpace : Dictionary<int, Space>
	{

		#region Constructors

		public DocumentSpace()
		{
		}

		#endregion

		#region Properties

		#endregion

		#region Methods

		/// <summary>
		/// If Dictionary contains <paramref name="key"/>, value is replaceed. 
		/// If Dictionary doesn't contain <paramref name="key"/> method creates it an assignt <paramref name="value"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Append(int key, Space value)
		{
			if(!ContainsKey(key))
				Add(key, value);
			else
				this[key] = value;
		}

		#endregion

		#region Helpers

		#endregion


		 
	}
}