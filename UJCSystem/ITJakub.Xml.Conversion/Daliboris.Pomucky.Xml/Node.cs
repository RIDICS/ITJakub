using System;
using System.Collections.Generic;
using System.Xml;

namespace Daliboris.Pomucky.Xml
{
	public class Node
	{

		#region Constructors

		public Node(int depth)
		{
			Depth = depth;
			Attributes = new Dictionary<string, string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public Node(int depth, string name)
			: this(depth)
		{
			Name = name;
		}


		#endregion

		#region Properties

		public string Name { get; set; }
		public Dictionary<string, string> Attributes { get; set; }
		public string Text { get; set; }
		public SpaceHandling SpaceHandling { get; set; }
		public int Depth { get; set; }
		public bool IsEmpty { get; set; }
		public bool EndsWithSpace
		{
			get
			{
				if (String.IsNullOrEmpty(Text)) return false;
				return (Text[Text.Length - 1] == ' ');
			}
		}

		#endregion

		#region Methods

		#endregion

		#region Helpers

		public static Node ReadNode(XmlReader reader)
		{
			Node node = null;
			if (reader.NodeType == XmlNodeType.Element)
			{
				node = new Node(reader.Depth, reader.Name);
				if (reader.IsEmptyElement)
					node.IsEmpty = true;
				if (reader.HasAttributes)
					while (reader.MoveToNextAttribute())
					{
						node.Attributes.Add(reader.Name, reader.Value);
					}

			}
			else if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.SignificantWhitespace)
			{
				node = new Node(reader.Depth);
				node.Text = reader.Value;
			}

			else
			{
				throw new InvalidOperationException("Aktuální prvek '" + reader.NodeType.ToString() + "' nelze převést na objekt Node.");
			}

			return node;
		}

		public static void Deserialize(XmlWriter writer, Stack<Node> previousNodes)
		{
			while (previousNodes.Count > 0)
			{
				Node node = previousNodes.Pop();
				Deserialize(writer, node);
			}
		}

		public static void Deserialize(XmlWriter writer, Node node)
		{
			if (node.Name == null)
			{
				
				writer.WriteString(node.Text);
			}
			else
			{
				writer.WriteStartElement(node.Name);
				foreach (KeyValuePair<string, string> attribute in node.Attributes)
				{
					writer.WriteAttributeString(attribute.Key, attribute.Value);
				}
				writer.WriteEndElement();
			}
				
		}

		#endregion



	}
}