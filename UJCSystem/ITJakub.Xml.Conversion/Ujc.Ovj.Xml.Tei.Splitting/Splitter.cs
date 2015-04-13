using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Daliboris.Pomucky.Xml;

namespace Ujc.Ovj.Xml.Tei.Splitting
{

	/// <summary>
	/// Třída sloužící k rozdělení souboru XML na samostatné části ohraničené stranami
	/// </summary>
	public class Splitter
	{

		const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
		const string TeiNamespace = "http://www.tei-c.org/ns/1.0";
		private const string ItJakubTeiNamespace = "http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0";

		const string NumberedXmlPattern = "_{0:0000}.xml";

		private XmlWriter currentWriter;


		private OutputManager outputManager;

		private SplittingResult result;

		private PageBreakSplitInfo currentSplitInfo;

		#region Vlastnosti
		/// <summary>
		/// Soubor XML, který se má rozdělit na menší části
		/// </summary>
		public string XmlFile { get; set; }

		/// <summary>
		/// Složka, do níž se uloží jednotlivé části rozděleného souboru
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// Počáteční element, od něhož začne rozdělování souboru na menší části
		/// </summary>
		public string StartingElement { get; set; }

		/// <summary>
		/// Pokud má hodnotu <value>true</value>, použe se Id elementu jako název generovaného souboru.
		/// </summary>
		public bool UseElementIdAsFileName { get; set; }

		#endregion

		#region Konstruktory
		public Splitter() { }

		public Splitter(string xmlFile, string outputDirectory)
		{
			XmlFile = xmlFile;
			OutputDirectory = outputDirectory;
		}

		#endregion

		#region Metody
		public void SplitOnStartingElement(string xmlFile, string outputDirectory)
		{
			XmlFile = xmlFile;
			OutputDirectory = outputDirectory;
			SplitOnStartingElement();
		}

		public void SplitOnStartingElement()
		{

			bool splittingStarted = (StartingElement == null);

			ElementInfos elementQueue = new ElementInfos();
			FileInfo xmlFileInfo = new FileInfo(XmlFile);


			string newFileFormat = xmlFileInfo.Name.Substring(0, xmlFileInfo.Name.Length - xmlFileInfo.Extension.Length) + NumberedXmlPattern;


			outputManager = new OutputManager();
			outputManager.OutputDirectory = OutputDirectory;
			outputManager.FileNameFormat = newFileFormat;



			using (XmlReader reader = XmlReader.Create(XmlFile))
			{
				reader.MoveToContent();
				while (reader.Read())
				{

					if (reader.NodeType == XmlNodeType.Element)
					{

						bool isEmpty = reader.IsEmptyElement;

						if (!splittingStarted)
							if (reader.Name == StartingElement)
							{
								splittingStarted = true;
								currentWriter = outputManager.GetXmlWriter();
								currentWriter.WriteStartDocument();
								currentWriter.WriteStartElement("fragment", TeiNamespace);
							}

						if (!splittingStarted)
							continue;

						ElementInfo element = GetElementInfo(reader);
						elementQueue.Push(element);
						WriteElementInfo(element, currentWriter);

						if (!isEmpty)
							continue;
						elementQueue.Pop();
					}

					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (!splittingStarted)
							continue;
						string name = reader.Name;
						ElementInfo elementPeak = elementQueue.Peek();
						if (elementPeak.Name != name)
						{
							Console.WriteLine("Chyba element × reader ({0} × {1})", elementPeak.Name, name);
						}
						else
						{
							Transformace.SerializeNode(reader, currentWriter);
							elementQueue.Pop();
						}
						if (name == StartingElement)
						{
							elementQueue.CloneReverse();

							while (elementQueue.Count > 0)
							{
								currentWriter.WriteEndElement();
								elementQueue.Pop();
							}
							currentWriter.WriteFullEndElement();
							currentWriter.Close();

							splittingStarted = false;
							//currentWriter = outputManager.GetXmlWriter();
						}
					}
					else
					{
						if (splittingStarted)
							Transformace.SerializeNode(reader, currentWriter);
					}
				}

			}
		}

		public SplittingResult SplitOnPageBreak()
		{

			result = new SplittingResult(XmlFile, OutputDirectory);

			bool splittingStarted = (StartingElement == null);

			ElementInfo startElement = null;

			ElementInfos elementStack = new ElementInfos();
			FileInfo xmlFileInfo = new FileInfo(XmlFile);




			string newFileFormat = xmlFileInfo.Name.Substring(0, xmlFileInfo.Name.Length - xmlFileInfo.Extension.Length) + NumberedXmlPattern;


			outputManager = new OutputManager();
			outputManager.OutputDirectory = OutputDirectory;
			outputManager.FileNameFormat = newFileFormat;

			string divId = null;
			int paragraphId = 0;

			try
			{
				using (XmlReader reader = XmlReader.Create(XmlFile))
				{
					reader.MoveToContent();
					while (reader.Read())
					{
						string name = reader.Name;
						if (reader.NodeType == XmlNodeType.Element)
						{

							bool isEmpty = reader.IsEmptyElement;

							if (!splittingStarted)
								if (ShouldSplittingStart(reader, startElement))
								{
									startElement = new ElementInfo(reader.LocalName);
									startElement.Depth = reader.Depth;
									splittingStarted = true;
									currentSplitInfo = new PageBreakSplitInfo();
								}

							if (!splittingStarted)
								continue;
							if (reader.Name != "pb")
							{
								ElementInfo element = GetElementInfo(reader);
								if (element.Name == "div")
								{
									foreach (AttributeInfo attribute in element.Attributes)
									{
										if (attribute.LocalName == "id")
											divId = attribute.Value;
									}
								}
								if (element.Name == "p" || element.Name == "l")
								{
									if (!element.Attributes.Exists(a => a.Prefix == "xml" && a.LocalName == "id"))
									{
										string id = divId + "." + element.Name + ++paragraphId;
										element.Attributes.Add(new AttributeInfo("xml", "id", XmlNamespace, id));
									}
								}


								if (currentWriter != null)
								{
									elementStack.Push(element);
									WriteElementInfo(element, currentWriter);

									if (isEmpty)
									{
										elementStack.Pop();
										//pokud je element prázdný, při jeho přečtení se XmlReader přesune na další prvek
										//goto Begin;
									}
								}


							}
							else
							{
								if (currentSplitInfo != null && currentSplitInfo.Number == null)
								{
									currentSplitInfo.Id = reader.GetAttribute("xml:id");
									currentSplitInfo.Number = reader.GetAttribute("n");
								}

								if (outputManager.CurrentChunk == 0)
								{
									StartNewSplit(elementStack, null);
								}
								else
								{
									//ukončit předchozí sekvenci
									ElementInfos tempQueue = CloseCurrentSplit(elementStack);
									StartNewSplit(elementStack, tempQueue);
								}
								Transformace.SerializeNode(reader, currentWriter);
								//goto Begin;
							}
						}

						else if (reader.NodeType == XmlNodeType.EndElement)
						{
							if (!splittingStarted || startElement == null)
								continue;

							if (reader.Name == "div")
							{
								divId = null;
								paragraphId = 0;
							}

							if (ShouldSplittingEnd(reader, startElement))
							{
								CloseCurrentSplit(elementStack);
								startElement = null;
								splittingStarted = false;
							}
							if (!splittingStarted)
							{
								result.IsSplitted = true;
								return result;
							}

							ElementInfo elementPeak = elementStack.Peek();
							if (elementPeak.Name != name)
							{
								result.Errors = String.Format("Chyba {0} × {1} (element × reader)", elementPeak.Name, name);
								//Console.WriteLine("Chyba {0} × {1} (element × reader)", elementPeak.Name, name);
							}
							else
							{
								Transformace.SerializeNode(reader, currentWriter);
								elementStack.Pop();
							}
						}
						else
						{
							if (splittingStarted && currentWriter != null)
								Transformace.SerializeNode(reader, currentWriter);
						}
					}
				}
				result.IsSplitted = true;
			}
			catch (Exception exception)
			{
				result.Errors = exception.Message;
			}
			finally
			{
				if(currentWriter != null)
					currentWriter.Close();
			}

			return result;

		}

		#region Zavření, otevření úseku
		private void StartNewSplit(ElementInfos elementQueue, IEnumerable<ElementInfo> tempQueue)
		{

			currentWriter = outputManager.GetXmlWriter();

			currentSplitInfo.FileName = outputManager.CurrentFileName;
			currentSplitInfo.FullPath = outputManager.CurrentFileFullPath;

			currentWriter.WriteStartDocument();
			currentWriter.WriteStartElement("vw", "fragment", ItJakubTeiNamespace);
			currentWriter.WriteAttributeString("xmlns", TeiNamespace);
			if (tempQueue == null)
				return;
			foreach (ElementInfo elementInfo in tempQueue)
			{
				ElementInfo newElementInfo = elementInfo.Clone();
				newElementInfo.Attributes.Add(new AttributeInfo("vw", "continue", ItJakubTeiNamespace, "true"));
				WriteElementInfo(newElementInfo, currentWriter);
				elementQueue.Push(elementInfo);
			}
		}

		private ElementInfos CloseCurrentSplit(ElementInfos elementQueue)
		{


			ElementInfos tempQueue = elementQueue.Clone();

			while (elementQueue.Count > 0)
			{
				currentWriter.WriteEndElement();
				elementQueue.Pop();
			}
			currentWriter.WriteEndElement(); //tj. </vw:fragment>
			currentWriter.WriteEndDocument();
			currentWriter.Close();
			elementQueue.Clear();

			result.PageBreaksSplitInfo.Add(currentSplitInfo);
			currentSplitInfo = new PageBreakSplitInfo();

			return tempQueue;


		}

		#endregion

		#region Informace o úseku

		private static bool ShouldSplittingEnd(XmlReader reader, ElementInfo startElement)
		{
			if (startElement == null)
				return false;
			if (reader.Name == startElement.Name && reader.Depth == startElement.Depth)
				return true;
			return false;
		}

		private bool ShouldSplittingStart(XmlReader reader, ElementInfo startElement)
		{
			if (StartingElement == null)
				return true;

			if (reader.LocalName == StartingElement)
			{
				if (startElement != null)
					return false;

				startElement = new ElementInfo(reader.LocalName);
				startElement.Depth = reader.Depth;

				return true;
			}
			return false;
		}

		#endregion
		#endregion

		#region ElementInfo manipulation

		private static void WriteElementInfo(ElementInfo element, XmlWriter writer)
		{
			writer.WriteStartElement(element.Name);
			foreach (AttributeInfo attribute in element.Attributes)
			{
				writer.WriteStartAttribute(attribute.Prefix, attribute.LocalName,
								attribute.NamespaceUri);
				writer.WriteString(attribute.Value);
				writer.WriteEndAttribute();

			}
			if (element.IsEmpty)
				writer.WriteEndElement();
		}

		private ElementInfo GetElementInfo(XmlReader reader)
		{
			if (reader.NodeType != XmlNodeType.Element)
				return null;

			ElementInfo element = new ElementInfo(reader.Name);
			element.IsEmpty = reader.IsEmptyElement;
			element.Depth = reader.Depth;
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
			return element;
		}

		#endregion
	}
}
