// -----------------------------------------------------------------------
// <copyright file="ElementInfos.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Ujc.Ovj.Xml.Info {
	/// <summary>
 /// TODO: Update summary.
 /// </summary>
 public class ElementInfos : Stack<ElementInfo>, ICloneable
 {

	public ElementInfos Clone()
	{
	 ElementInfos elementInfos = new ElementInfos();
	 ElementInfo[] elementArray = this.ToArray();
	 for (int i = 0; i < elementArray.Length; i++) {
		elementInfos.Push(elementArray[i]);
	 }
	 return elementInfos;
	}

	/// <summary>
	/// Vrací klon fronty v opačném pořadí prvků
	/// </summary>
	/// <returns></returns>
	public ElementInfos CloneReverse()
	{
	 ElementInfos elementInfos = new ElementInfos();
	 ElementInfo[] elementArray = this.ToArray();
		for (int i = elementArray.Length - 1; i >= 0; i--)
		{
		 elementInfos.Push(elementArray[i]);	
		}
	 return elementInfos;
	}


 	object ICloneable.Clone()
 	{
 		return Clone();
 	}
 }
}
