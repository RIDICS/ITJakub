// -----------------------------------------------------------------------
// <copyright file="ElementInfo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Xml;


namespace Ujc.Ovj.Xml.Tei.Splitting {
 using System;
 using System.Collections.Generic;
 using System.Text;



 /// <summary>
 /// TODO: Update summary.
 /// </summary>
 [System.Diagnostics.DebuggerDisplay("{Name}, {Attributes.Count} (Depth: {Depth}, Empty: {IsEmpty})")]
 public class ElementInfo : ICloneable {

 	public ElementInfo(string name)
 	{
 		Name = name;
 	}
	
 	public string Name { get; set; }
 	private List<AttributeInfo> attributes = new List<AttributeInfo>();

 	public List<AttributeInfo> Attributes
 	{
 		get { return attributes; }
 		set { attributes = value; }
 	}

 	public int Depth { get; set; }
	public bool IsEmpty { get; set; }

	public ElementInfo Clone()
	{
		ElementInfo element = (ElementInfo) this.MemberwiseClone();
	 element.Attributes = new List<AttributeInfo>(Attributes);
	 return element;
	}

 	object ICloneable.Clone()
 	{
 		return Clone();
 	}
 }


}
