// -----------------------------------------------------------------------
// <copyright file="ElementInfo.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;

namespace Ujc.Ovj.Xml.Info
{
	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("{Name}, {Attributes.Count} (Depth: {Depth}, Empty: {IsEmpty})")]
	public class ElementInfo : ICloneable
	{

		public ElementInfo(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
		private AttributeInfos attributes = new AttributeInfos();

		public AttributeInfos Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		public int Depth { get; set; }
		public bool IsEmpty { get; set; }

		public ElementInfo Clone()
		{
			ElementInfo element = (ElementInfo)this.MemberwiseClone();
			element.Attributes = new AttributeInfos(Attributes);
			return element;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public static ElementInfo GetElementInfo(XmlReader reader)
		{
			if(!(reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.EndElement))
				return null;

			ElementInfo element = new ElementInfo(reader.Name);
			element.Depth = reader.Depth;
			if (reader.NodeType == XmlNodeType.EndElement)
				return element;
			element.IsEmpty = reader.IsEmptyElement;
			if (reader.HasAttributes)
			{
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					AttributeInfo attribute = new AttributeInfo();
					attribute.Prefix = reader.Prefix;
					attribute.LocalName = reader.LocalName;
					attribute.NamespaceUri = reader.NamespaceURI;
					attribute.Value = reader.Value;

					element.Attributes.Add(attribute);
				}
				reader.MoveToElement();
			}
			return element;
		}
	}



}
