using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Xml;
using System.Collections.Generic;
//using System.Linq;

namespace Daliboris.Pomucky.Xml
{
	public static partial class  Transformace
	{
		public static string ZnakyInterpunkce = ".,;:?!…„“";

		/// <summary>
		/// Překopíruje obsah uzlu do nového dokumentu.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má překopírovat.</param>
		/// <param name="w">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		public static void SerializeNode(XmlReader reader, XmlWriter w)
		{
			SerializeNode(reader, w, false);
		}

		/// <summary>
		/// Překopíruje obsah uzlu do nového dokumentu.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má překopírovat.</param>
		/// <param name="w">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="blnVypisovatVychoziHodnoty">Zda se mají (true), nebo nemají (false) vypisovat výchozí hodnoty atributů, pokud nejsou uvedeny ve zdrojovém dokumentu a jsou definovány v DTD nebo XSD.</param>
		public static void SerializeNode(XmlReader reader, XmlWriter w, Boolean blnVypisovatVychoziHodnoty)
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.Element:
					w.WriteStartElement(reader.Prefix, reader.LocalName,
							reader.NamespaceURI);
					bool bEmpty = reader.IsEmptyElement;
					bool hasAttributes = reader.HasAttributes;
					while (reader.MoveToNextAttribute())
					{
						if (blnVypisovatVychoziHodnoty)
						{
							w.WriteStartAttribute(reader.Prefix, reader.LocalName,
									reader.NamespaceURI);
							w.WriteString(reader.Value);
							w.WriteEndAttribute();
						}
						else
						{
							if (!reader.IsDefault)
							{
								w.WriteStartAttribute(reader.Prefix, reader.LocalName,
										reader.NamespaceURI);
								w.WriteString(reader.Value);
								w.WriteEndAttribute();

							}
						}
					}
					if (bEmpty)
					{
						w.WriteEndElement();
						if (hasAttributes)
							reader.MoveToElement(); //je třeba vrátit se k elementu, aby se správně nastavilo zanoření elementu (atributy mají zanoření o stupeň nižší)
						
					}
					break;
				case XmlNodeType.Text:
					w.WriteString(reader.Value);
					break;
				case XmlNodeType.CDATA:
					w.WriteCData(reader.Value);
					break;
				case XmlNodeType.XmlDeclaration:
				case XmlNodeType.ProcessingInstruction:
					if (reader.Name == "xml")
					{
						if (w.WriteState == WriteState.Start)
							w.WriteProcessingInstruction(reader.Name, reader.Value);
					}
					else
						w.WriteProcessingInstruction(reader.Name, reader.Value);
					break;
				case XmlNodeType.Comment:
					w.WriteComment(reader.Value);
					break;
				case XmlNodeType.EndElement:
					//w.WriteEndElement();
					w.WriteFullEndElement();
					break;
				case XmlNodeType.Attribute:
					//pokud atribut definuje výchozí namespace
					if (reader.LocalName == "" && reader.Prefix == "xmlns")
					{
						//w.WriteStartAttribute(null, reader.LocalName, reader.NamespaceURI);
						w.WriteAttributeString("xmlns", null, null, reader.Value);
					}
					else
					{
						try
						{
							w.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
							w.WriteString(reader.Value);
							w.WriteEndAttribute();
						}
						catch (System.Xml.XmlException exception)
						{
							//nejspíš se jedná o duplicitní atribut
							//if(exception.GetObjectData())
						}
					}
					break;
				case XmlNodeType.EntityReference:
					w.WriteEntityRef(reader.Name);
					break;
				case XmlNodeType.DocumentType:
					// Get the public and system IDs to pass to the WriteDocType method
					w.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					w.WriteWhitespace(reader.Value);
					break;
			}


		}

		/// <summary>
		/// Překopíruje celý uzel (až do koncového elementu) do nového dokumentu
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má překopírovat.</param>
		/// <param name="w">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		public static void SerializeWholeNode(XmlReader reader, XmlWriter w)
		{
			SerializeWholeNode(reader, w, false);
		}

		/// <summary>
		/// Překopíruje celý uzel (až do koncového elementu) do nového dokumentu
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má překopírovat.</param>
		/// <param name="w">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="includingEndElement"></param>
		public static void SerializeWholeNode(XmlReader reader, XmlWriter w, bool includingEndElement)
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.Element:
					bool isEmpty = reader.IsEmptyElement;

					if (isEmpty)
					{
						SerializeNode(reader, w);
						return;
					}

					string sElement = reader.Name;
					while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == sElement))
					{
						SerializeNode(reader, w);
						reader.Read();
					}
					if (includingEndElement)
					{
						if (reader.NodeType == XmlNodeType.EndElement && reader.Name == sElement)
						{
							SerializeNode(reader, w);
							//reader.Read();
						}
					}
					break;
				default:
					SerializeNode(reader, w);
					break;
			}
		}


		/// <summary>
		/// Překopíruje atributy aktuálního uzlu do nového dokumentu.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, jehož atributy se mají pžřekopírovat.</param>
		/// <param name="w">XmlWriter nového dokumentu, do něhož se mají atributy překopírovat.</param>
		/// <param name="blnVypisovatVychoziHodnoty">Zda se mají (true), nebo nemají (false) vypisovat výchozí hodnoty atributů, pokud nejsou uvedeny ve zdrojovém dokumentu a jsou definovány v DTD nebo XSD.</param>
		public static void SerializeAttributes(XmlReader reader, XmlWriter w, Boolean blnVypisovatVychoziHodnoty)
		{
			while (reader.MoveToNextAttribute())
			{
				if (blnVypisovatVychoziHodnoty)
				{
					w.WriteStartAttribute(reader.Prefix, reader.LocalName,
							reader.NamespaceURI);
					w.WriteString(reader.Value);
					w.WriteEndAttribute();
				}
				else
				{
					if (!reader.IsDefault)
					{
						w.WriteStartAttribute(reader.Prefix, reader.LocalName,
								reader.NamespaceURI);
						w.WriteString(reader.Value);
						w.WriteEndAttribute();

					}
				}
			}

		}


		public static void PridatMezeryZaTagyPoInterpunkci(string vstup, string vystup,
			XmlReaderSettings xmlReaderSettings, List<string> tagyPoInterpunkci, string znakyInterpunkce)
		{
			Console.WriteLine("{0} => {1}, Přidat mezery za tagy po interpunkci", vstup, vystup);
			using (XmlReader xr = XmlReader.Create(vstup, xmlReaderSettings))
			{
				using (XmlWriter xw = XmlWriter.Create(vystup))
				{
					PridatMezeryZaTagyPoInterpunkci(xr, xw, tagyPoInterpunkci, znakyInterpunkce);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="writer"></param>
		/// <param name="tagyPoInterpunkci">Tagy, po nich se má vložit mezera, pokud za nimi následuje interpunkce.</param>
		/// <param name="znakyInterpunkce">Znaky, které jsou považovány za interpunkci.</param>
		private static void PridatMezeryZaTagyPoInterpunkci(XmlReader reader, XmlWriter writer, 
			List<string> tagyPoInterpunkci, string znakyInterpunkce)
		{
			List<char> interpunkce = new List<char>(znakyInterpunkce.ToCharArray());

			Queue<string> qsTagy = new Queue<string>();
			bool vlozitMezeru = false;
			bool bylaInterpunkce = false;
			//Queue<string> qsTexty = new Queue<string>();


			string mstrAktualniText = null;
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						if (tagyPoInterpunkci.Contains(reader.Name))
						{
							SerializeWholeNode(reader, writer, true);
							break;
						}
						if (bylaInterpunkce && !tagyPoInterpunkci.Contains(reader.Name))
							vlozitMezeru = false;
						SerializeNode(reader, writer);
						break;
					case XmlNodeType.Text:
					case XmlNodeType.SignificantWhitespace:
						mstrAktualniText = reader.Value;
						if (vlozitMezeru)
						{
							writer.WriteString(" ");
							vlozitMezeru = false;
							bylaInterpunkce = false;
						}

						if (interpunkce.Contains(mstrAktualniText[mstrAktualniText.Length - 1]))
						{
							bylaInterpunkce = true;
						}
						//else
						SerializeNode(reader, writer);
						break;
					case XmlNodeType.EndElement:
						if (bylaInterpunkce && tagyPoInterpunkci.Contains(reader.Name))
						{
							vlozitMezeru = true;
						}
						else
						{
							vlozitMezeru = false;
							bylaInterpunkce = false;
						}
						SerializeNode(reader, writer);
						break;
					case XmlNodeType.XmlDeclaration:
						SerializeNode(reader, writer);
						writer.WriteComment("PridatMezeryZaTagyPoInterpunkci");
						break;
					default:
						SerializeNode(reader, writer);
						break;
				}
			}
		}

	}

	public class XmlElementInfo : ICloneable
	{
		internal const char Delimiter = '|';
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public XmlElementInfo(string name, int depth, bool isElementEmpty)
		{
			Name = name;
			Depth = depth;
			IsElementEmpty = isElementEmpty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public XmlElementInfo(string name, int depth, bool isElementEmpty, string ns) : this(name, depth, isElementEmpty)
		{
			Namespace = ns;
		}

		public string Name { get; set; }
		public int Depth { get; set; }
		public string Namespace { get; set; }
		public bool IsElementEmpty { get; set; }
		public bool ContainsText { get; set; }

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			if (Namespace != null)
			{
				return String.Format("{1}{0}{2}{0}{3}", Delimiter, Name, Depth, Namespace);

			}
			return String.Format("{1}{0}{2}", Delimiter, Name, Depth);
		}

		private XmlElementInfo CloneImpl()
		{
			return this.MemberwiseClone() as XmlElementInfo;
			
		}

		public XmlElementInfo Clone()
		{
			return CloneImpl();
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		object ICloneable.Clone()
		{
			return CloneImpl();
		}

		#endregion

	}

}
