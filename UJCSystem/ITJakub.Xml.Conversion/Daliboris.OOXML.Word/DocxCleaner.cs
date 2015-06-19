using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Daliboris.Pomucky.Xml;

namespace Daliboris.OOXML.Word
{
	public class DocxCleaner
	{
		private XmlReader xmlReader;
		private XmlWriter xmlWriter;


		public DocxCleaner()
		{

		}

		public DocxCleaner(DocxCleanerSettings settings)
		{
			Settings = settings;
		}

		public DocxCleanerSettings Settings { get; private set; }

		public CleaningResult Clean(string document)
		{
			CleaningResult result = new CleaningResult();

			string temp = Path.GetTempFileName();

			bool zpracovat = true;

			Queue<RunObject> runs = new Queue<RunObject>();

			RunObject previous = null;

			using (xmlReader = XmlReader.Create(document))
			{
				XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlReader.NameTable);

				nsmgr.AddNamespace("w", Pomucky.Dokument.RelWordprocessingRelationshipTypeW);

				using (xmlWriter = XmlWriter.Create(temp))
				{
					xmlWriter.WriteStartDocument();

					while (xmlReader.Read())
					{
						string nazev = xmlReader.Name;
					Zacatek:
						if (xmlReader.NodeType == XmlNodeType.Element)
						{
							if (!zpracovat)
								break;
							switch (nazev)
							{
								case "w:r":
									RunObject actual = new RunObject();
									XmlDocument xd = Objekty.ReadNodeAsXmlDocument(xmlReader);
									actual.Xml = xd;
									XmlNode node = xd.SelectSingleNode("/w:r/w:rPr[1]/w:rStyle/@w:val", nsmgr);
									if (node == null)
										actual.Style = "Standardní písmo odstavce";
									else
										actual.Style = node.Value;
									XmlNode actualT = actual.Xml.SelectSingleNode("/w:r/w:t[1]", nsmgr);
									if (actualT == null) //jde o případy obrázku v dokumentu
									{
										if (previous != null) previous.Xml.Save(xmlWriter);
										actual.Xml.Save(xmlWriter);
										actual = previous = null;
									}
									if (previous != null)
									{
										if (previous.Style == actual.Style)
										{
											XmlNode prevT = previous.Xml.SelectSingleNode("/w:r/w:t[1]", nsmgr);
											prevT.InnerText += actualT.InnerText;
											if (prevT.InnerText[prevT.InnerText.Length - 1] == ' ' && prevT.Attributes["xml:space"] == null)
											{
												XmlAttribute at = previous.Xml.CreateAttribute("xml", "space", Objekty.XmlNamespace);
												at.Value = "preserve";
												prevT.Attributes.Append(at);
											}
											result.NumerOfChanges++;
										}
										else
										{
											previous.Xml.Save(xmlWriter);
											previous = actual;
										}
									}
									else
									{
										previous = actual;
									}
									goto Zacatek;
								default:
									Transformace.SerializeNode(xmlReader, xmlWriter);
									break;
							}
						}
						else if (xmlReader.NodeType == XmlNodeType.EndElement)
						{
							if(nazev == "w:p")
							{
								if(previous != null)
									previous.Xml.Save(xmlWriter);
								previous = null;
							}
								Transformace.SerializeNode(xmlReader, xmlWriter);
						}
						else
						{
							Transformace.SerializeNode(xmlReader, xmlWriter);
						}
					}

					xmlWriter.WriteEndDocument();
					xmlWriter.Close();
				}
			}

			result.Output = temp;
			result.Success = true;
			return result;
		}



	}
}
