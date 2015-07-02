using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Daliboris.Pomucky.Xml;
using Ujc.Ovj.Xml.Info;

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
		const string ErrorInfoFormat = "Chyba {0} × {1} (element × reader)";
		const string VwNamespacePrefix = "vw";
		const string XmlnsAttributeName = "xmlns";
		const string ContinueAttributeName = "continue";
		const string TrueValue = "true";

		const string NAttributeName = "n";
		const string XmlIdAttributeName = "xml:id";
		const string DivElementName = "div";
		const string PbElementName = "pb";
		const string IdAttributeName = "id";
		const string XmlNamespacePrefix = "xml";
		const string PElementName = "p";
		const string LElementName = "l";
		const string FragmentElementlName = "fragment";

		readonly List<string> _divElementNames  = new List<string>() {DivElementName};
		readonly List<string> _blockElementNames = new List<string>() { PElementName, LElementName };


		const string NumberedXmlPattern = "_{0:0000}.xml";

		private XmlWriter _currentWriter;


		private OutputManager _outputManager;

		private SplittingResult _result;

		private PageBreakSplitInfo _currentSplitInfo;

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

			ElementInfos elementStack = new ElementInfos();
			FileInfo xmlFileInfo = new FileInfo(XmlFile);


			string newFileFormat = xmlFileInfo.Name.Substring(0, xmlFileInfo.Name.Length - xmlFileInfo.Extension.Length) + NumberedXmlPattern;


			_outputManager = new OutputManager();
			_outputManager.OutputDirectory = OutputDirectory;
			_outputManager.FileNameFormat = newFileFormat;



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
								_currentWriter = _outputManager.GetXmlWriter();
								_currentWriter.WriteStartDocument();
								_currentWriter.WriteStartElement(FragmentElementlName, TeiNamespace);
							}

						if (!splittingStarted)
							continue;

						ElementInfo element = ElementInfo.GetElementInfo(reader); //GetElementInfo(reader);
						elementStack.Push(element);
						WriteElementInfo(element, _currentWriter);

						if (!isEmpty)
							continue;
						elementStack.Pop();
					}

					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (!splittingStarted)
							continue;
						string name = reader.Name;
						ElementInfo elementPeak = elementStack.Peek();
						if (elementPeak.Name != name)
						{
							Console.WriteLine("Chyba element × reader ({0} × {1})", elementPeak.Name, name);
						}
						else
						{
							Transformace.SerializeNode(reader, _currentWriter);
							elementStack.Pop();
						}
						if (name == StartingElement)
						{
							elementStack.CloneReverse();

							while (elementStack.Count > 0)
							{
								_currentWriter.WriteEndElement();
								elementStack.Pop();
							}
							_currentWriter.WriteFullEndElement();
							_currentWriter.Close();

							splittingStarted = false;
							//currentWriter = outputManager.GetXmlWriter();
						}
					}
					else
					{
						if (splittingStarted)
							Transformace.SerializeNode(reader, _currentWriter);
					}
				}

			}
		}

		public SplittingResult SplitOnPageBreak()
		{


			_result = new SplittingResult(XmlFile, OutputDirectory);

			bool splittingStarted = (StartingElement == null);

			ElementInfo startElement = null;

			ElementInfos elementStack = new ElementInfos();
			FileInfo xmlFileInfo = new FileInfo(XmlFile);




			string newFileFormat = xmlFileInfo.Name.Substring(0, xmlFileInfo.Name.Length - xmlFileInfo.Extension.Length) + NumberedXmlPattern;


			_outputManager = new OutputManager();
			_outputManager.OutputDirectory = OutputDirectory;
			_outputManager.FileNameFormat = newFileFormat;

			string divId = null;
			int paragraphId = 0;

			try
			{
				using (XmlReader reader = XmlReader.Create(XmlFile))
				{
					reader.MoveToContent();
					while (reader.Read())
					{
						switch (reader.NodeType)
						{
							case XmlNodeType.Element:
								ElementInfo element = ElementInfo.GetElementInfo(reader);

								if (!splittingStarted)
								{
									if (!ShouldSplittingStart(element, startElement)) continue;
								}
								if (!splittingStarted)
								{
									startElement = element.Clone();
									splittingStarted = true;
									_currentSplitInfo = new PageBreakSplitInfo();
								}

								if (element.Name == PbElementName)
								{

									if (_currentSplitInfo != null && _currentSplitInfo.Number == null)
									{
										_currentSplitInfo.Id = element.Attributes.GetAttributeByLocalName(XmlNamespacePrefix, IdAttributeName).Value;
										_currentSplitInfo.Number = element.Attributes.GetAttributeByLocalName(String.Empty, NAttributeName).Value;
									}

									ElementInfos tempQueue = null;
									if (_outputManager.CurrentChunk > 0)
									{
										tempQueue = CloseCurrentSplit(elementStack);
									}
									StartNewSplit(elementStack, tempQueue);
									_currentSplitInfo.Number = element.Attributes.GetAttributeByLocalName(String.Empty, NAttributeName).Value;
									_currentSplitInfo.Id = element.Attributes.GetAttributeByLocalName(XmlNamespacePrefix, IdAttributeName).Value;

									Transformace.SerializeNode(reader, _currentWriter);
									//goto Begin;
								}
								else  //(reader.Name == "pb")
								{
									if (_divElementNames.Contains(element.Name))
									{
										foreach (AttributeInfo attribute in element.Attributes)
										{
											if (attribute.LocalName == IdAttributeName)
												divId = attribute.Value;
										}
									}
									if (_blockElementNames.Contains(element.Name))
									{
										if (!element.Attributes.Exists(a => a.Prefix == XmlNamespacePrefix && a.LocalName == IdAttributeName))
										{
											string id = divId + "." + element.Name + ++paragraphId;
											element.Attributes.Add(new AttributeInfo(XmlNamespacePrefix, IdAttributeName, XmlNamespace, id));
										}
									}
									if (_currentWriter != null)
									{
										if (!element.IsEmpty)
											elementStack.Push(element);
										//pokud je element prázdný, při jeho přečtení se XmlReader přesune na další prvek
										WriteElementInfo(element, _currentWriter);
									}
								}
								break;
							case XmlNodeType.EndElement:
								ElementInfo endElementInfo = ElementInfo.GetElementInfo(reader);

								if (!splittingStarted || startElement == null)
									continue;

								if (_divElementNames.Contains(reader.Name))
								{
									divId = null;
									paragraphId = 0;
								}

								if (ShouldSplittingEnd(endElementInfo, startElement))
								{
									CloseCurrentSplit(elementStack);
									_result.IsSplitted = true;
									return _result;
								}

								if (elementStack.Count > 0)
								{
									ElementInfo elementPeak = elementStack.Peek();
									if (elementPeak.Name != endElementInfo.Name)
									{
										_result.Errors = String.Format(ErrorInfoFormat, elementPeak.Name, endElementInfo.Name);
										//Console.WriteLine("Chyba {0} × {1} (element × reader)", elementPeak.Name, name);
									}
									else
									{
										Transformace.SerializeNode(reader, _currentWriter);
										elementStack.Pop();
									}
								}
								break;
							default:
								if (splittingStarted && _currentWriter != null)
									Transformace.SerializeNode(reader, _currentWriter);
								break;
						}
					}
				}
				_result.IsSplitted = true;
			}
			catch (Exception exception)
			{
				_result.Errors = exception.Message;
			}
			finally
			{
				if (_currentWriter != null)
					_currentWriter.Close();
			}

			return _result;

		}

		#region Zavření, otevření úseku
		private void StartNewSplit(ElementInfos elementQueue, IEnumerable<ElementInfo> tempQueue)
		{

			_currentWriter = _outputManager.GetXmlWriter();

			_currentSplitInfo.FileName = _outputManager.CurrentFileName;
			_currentSplitInfo.FullPath = _outputManager.CurrentFileFullPath;

			_currentWriter.WriteStartDocument();
			_currentWriter.WriteStartElement(VwNamespacePrefix, FragmentElementlName, ItJakubTeiNamespace);
			_currentWriter.WriteAttributeString(XmlnsAttributeName, TeiNamespace);
			if (tempQueue == null)
				return;
			foreach (ElementInfo elementInfo in tempQueue)
			{
				ElementInfo newElementInfo = elementInfo.Clone();

				newElementInfo.Attributes.Add(new AttributeInfo(VwNamespacePrefix, ContinueAttributeName, ItJakubTeiNamespace, TrueValue));
				WriteElementInfo(newElementInfo, _currentWriter);
				elementQueue.Push(elementInfo);
			}
		}

		private ElementInfos CloseCurrentSplit(ElementInfos elementQueue)
		{


			ElementInfos tempQueue = elementQueue.Clone();

			while (elementQueue.Count > 0)
			{
				_currentWriter.WriteEndElement();
				elementQueue.Pop();
			}
			if (_currentWriter != null)
			{
				_currentWriter.WriteEndElement(); //tj. </vw:fragment>
				_currentWriter.WriteEndDocument();
				_currentWriter.Close();
			}
			elementQueue.Clear();

			if (_currentSplitInfo != null && _currentSplitInfo.Id != null)
			{
				_result.PageBreaksSplitInfo.Add(_currentSplitInfo);
				_currentSplitInfo = new PageBreakSplitInfo();
			}

			return tempQueue;


		}

		#endregion

		#region Informace o úseku

		private static bool ShouldSplittingEnd(ElementInfo currentElement, ElementInfo startElement)
		{
			if (startElement == null)
				return false;
			if (currentElement.Name == startElement.Name && currentElement.Depth == startElement.Depth)
				return true;
			return false;
		}

		private bool ShouldSplittingStart(ElementInfo currentElement, ElementInfo startElement)
		{
			if (StartingElement == null)
				return true;

			if (currentElement.Name == StartingElement)
			{
				if (startElement != null)
					return false;

				startElement = new ElementInfo(currentElement.Name);
				startElement.Depth = currentElement.Depth;

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

		#endregion
	}
}
