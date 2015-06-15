using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Xml;

namespace Daliboris.Pomucky.Xml
{
	public static partial class Transformace
	{
		#region Přesun mezery vně tagu


		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="vynechaneTagy">Seznam tagů, před nimiž se mezera nepřesouvá.</param>
		/// <param name="preskoceneTagy">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		public static void PresunoutMezeryVneTagu(XmlReader reader, XmlWriter writer, List<string> vynechaneTagy, List<string> preskoceneTagy)
		{
			Queue<string> qsTagy = new Queue<string>();
			Stack<string> predchoziTagy = new Stack<string>();

			//Queue<string> qsTexty = new Queue<string>();
			if (vynechaneTagy == null)
				vynechaneTagy = new List<string>();
			if (preskoceneTagy == null)
				preskoceneTagy = new List<string>();
			VyclenenaMezera mezera = new VyclenenaMezera();


			string mstrAktualniText = null;
			while (reader.Read())
			{

			Zacatek:
				string readerName = reader.Name;
				if (reader.NodeType == XmlNodeType.Element)
				{
					//pokud nastane kombinace
					//"text <supplied>... </supplied><note>poznámka</note>text" =>
					//"text <supplied>...</supplied><note>poznámka</note> text"
					if (preskoceneTagy.Contains(readerName))
					{
						writer.WriteNode(reader, false);
						goto Zacatek;
					}

					if (!reader.IsEmptyElement)
						predchoziTagy.Push(readerName);

					if (mezera.ZapsatMezeru)
					{
						if (reader.IsEmptyElement || vynechaneTagy.Contains(readerName))
						//u prázdných elementů se mezera za element neposouvá
						{
							writer.WriteString(" ");
							mezera.ZapsatMezeru = false;
							//if (reader.IsEmptyElement)
							//    predchoziTagy.Pop();
						}
						else // nejede-li o prázdný element, zaznamená se jeho jméno do seznamu
							qsTagy.Enqueue(readerName);
						if (qsTagy.Count == 1)
						{
							mezera.ZapsatMezeru = false;
						}
						if (mezera.ZapsatMezeru)
							if (predchoziTagy.Count <= mezera.Depth)
								mezera.ZapsatMezeru = false;
					}
					SerializeNode(reader, writer);
				}
				else if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.SignificantWhitespace)
				{
					mstrAktualniText = reader.Value;
					//mstrAktualniText = mstrAktualniText.Replace(" ,", ",").Replace(" .", ".").Replace(" !", "!").Replace(" ?", "?");

					//if(pouzeZapsatMezeru)
					if (mezera.ZapsatMezeru)
					{
						writer.WriteString(" ");
						mezera.ZapsatMezeru = false;
					}

					if (mstrAktualniText[mstrAktualniText.Length - 1] == ' ')
					{
						mezera = new VyclenenaMezera(reader.Depth, predchoziTagy.Peek());
						mezera.ZapsatMezeru = true;
						writer.WriteString(mstrAktualniText.Remove(mstrAktualniText.Length - 1));
					}
					else
						SerializeNode(reader, writer);
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					string sTag = readerName;

					if (mezera.ZapsatMezeru)
					{
						if (predchoziTagy.Count == mezera.Depth && predchoziTagy.Peek() == mezera.Tag)
						{
							if (!vynechaneTagy.Contains(predchoziTagy.Peek()))
							{
								mezera.ZapsatMezeru = false;
							}

						}
						if (predchoziTagy.Count < mezera.Depth)
							mezera.ZapsatMezeru = false;
					}

					SerializeNode(reader, writer);
					if (predchoziTagy.Count > 0)
						predchoziTagy.Pop();


					if (qsTagy.Count > 0)
					{
						string sPrevTag = qsTagy.Peek();
						if (sPrevTag == sTag)
						{
							writer.WriteString(" ");
							qsTagy.Dequeue();
						}
						else
						{
							//if (pouzeZapsatMezeru)
							if (mezera.ZapsatMezeru)
							{ //mezera se vyskytla v rámci jednoho tagu <supplied>text </supplied>
								try
								{
									writer.WriteString(" ");
									mezera.ZapsatMezeru = false;
								}
								catch (Exception e)
								{
									if (!e.Message.Contains("EndRootElement"))
										throw;
								}

							}
						}
					}
					//pro případy typu
					//<head><pb n="FS" rend="space"/><foreign>Levitici XI </foreign><note place="bottom">Lv 11,5–30</note></head>
					//mezera se nesmí přesunout za </head>
					//xxxx
					//if (mezera.Depth >= reader.Depth)
					//{
					//    //if(reader.Depth > 0)
					//    //    writer.WriteString(" ");
					//    mezera.ZapsatMezeru = false;
					//}
					//else if (vlozitMezeru)
					//{ //neexistují žádné uložené tagy, ale existuje mezera
					//	writer.WriteString(" ");
					//	vlozitMezeru = false;
					//}
					//if (vlozitMezeru)
					//	vlozitMezeru = false;
				}
				else
				{
					SerializeNode(reader, writer);
				}
			}
		}


		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="tagyPredNimizSeMezeraNepresouva">Seznam tagů, před nimiž se mezera nepřesouvá.</param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		public static void PresunoutMezeryVneTagu0(XmlReader reader, XmlWriter writer, List<string> tagyPredNimizSeMezeraNepresouva,
			List<string> tagyKdeSeMezeraPresouvaZaZnacku)
		{
			Queue<string> qsTagy = new Queue<string>();
			Stack<string> predchoziTagy = new Stack<string>();

			//Queue<string> qsTexty = new Queue<string>();
			if (tagyPredNimizSeMezeraNepresouva == null)
				tagyPredNimizSeMezeraNepresouva = new List<string>();
			if (tagyKdeSeMezeraPresouvaZaZnacku == null)
				tagyKdeSeMezeraPresouvaZaZnacku = new List<string>();
			VyclenenaMezera mezera = new VyclenenaMezera();


			string mstrAktualniText = null;
			while (reader.Read())
			{

			Zacatek:
				string readerName = reader.Name;
				if (reader.NodeType == XmlNodeType.Element)
				{
					//pokud nastane kombinace
					//"text <supplied>... </supplied><note>poznámka</note>text" =>
					//"text <supplied>...</supplied><note>poznámka</note> text"
					if (tagyKdeSeMezeraPresouvaZaZnacku.Contains(readerName))
					{
						mezera.JePreskocenTag = true;
						writer.WriteNode(reader, false);
						goto Zacatek;
					}

					mezera.JePreskocenTag = false;

					if (!reader.IsEmptyElement)
						predchoziTagy.Push(readerName);

					if (mezera.ZapsatMezeru)
					{
						if (reader.IsEmptyElement)
						//u prázdných elementů se mezera za element neposouvá
						{
							if (mezera.JakoAtribut && mezera.Depth <= reader.Depth)
							{
								mezera.JakoAtribut = false;
							}
							else
							{
								writer.WriteString(" ");
							}
							mezera.ZapsatMezeru = false;
							//if (reader.IsEmptyElement)
							//    predchoziTagy.Pop();
						}
						else if (tagyPredNimizSeMezeraNepresouva.Contains(readerName))
						{
							qsTagy.Enqueue(readerName);
						}
						else // nejede-li o prázdný element, zaznamená se jeho jméno do seznamu
							qsTagy.Enqueue(readerName);
						if (qsTagy.Count == 1)
						{
							if (!tagyPredNimizSeMezeraNepresouva.Contains(readerName))
								mezera.ZapsatMezeru = false;
						}
						if (mezera.ZapsatMezeru)
							if (predchoziTagy.Count <= mezera.Depth)
								if (!tagyPredNimizSeMezeraNepresouva.Contains(readerName))
									mezera.ZapsatMezeru = false;
					}

					//TODO Generalizovat výjimku, kdy je mezera specifikovaná pomocí hodnoty atributu
					if (reader.HasAttributes)
					{
						if (reader.GetAttribute("rend") == "space")
						{
							mezera = new VyclenenaMezera(reader.Depth, readerName, true);
						}
					}
					SerializeNode(reader, writer);
				}
				else if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.SignificantWhitespace)
				{
					mstrAktualniText = reader.Value;
					//mstrAktualniText = mstrAktualniText.Replace(" ,", ",").Replace(" .", ".").Replace(" !", "!").Replace(" ?", "?");

					//if(pouzeZapsatMezeru)
					if (mezera.ZapsatMezeru)
					{
						if (!mezera.JePreskocenTag && mezera.JakoAtribut && mezera.Depth <= reader.Depth)
						{
							mezera.ZapsatMezeru = false;
						}
						else
						{
							writer.WriteString(" ");
							mezera.ZapsatMezeru = false;
						}
					}

					if (mstrAktualniText[mstrAktualniText.Length - 1] == ' ')
					{
						mezera = new VyclenenaMezera(reader.Depth, predchoziTagy.Peek());
						mezera.ZapsatMezeru = true;
						writer.WriteString(mstrAktualniText.Remove(mstrAktualniText.Length - 1));
					}
					else
						SerializeNode(reader, writer);
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					string sTag = readerName;

					if (mezera.ZapsatMezeru)
					{
						if (predchoziTagy.Count == mezera.Depth && predchoziTagy.Peek() == mezera.Tag)
						{
							if (!tagyPredNimizSeMezeraNepresouva.Contains(predchoziTagy.Peek()))
							{
								mezera.ZapsatMezeru = mezera.JakoAtribut = false;
							}

						}
						if (predchoziTagy.Count < mezera.Depth)
							mezera.ZapsatMezeru = mezera.JakoAtribut = false;
					}

					SerializeNode(reader, writer);
					if (predchoziTagy.Count > 0)
						predchoziTagy.Pop();


					if (qsTagy.Count > 0)
					{
						string sPrevTag = qsTagy.Peek();
						if (sPrevTag == sTag)
						{
							writer.WriteString(" ");
							qsTagy.Dequeue();
						}
						else
						{
							//if (pouzeZapsatMezeru)
							if (mezera.ZapsatMezeru)
							{
								//mezera se vyskytla v rámci jednoho tagu <supplied>text </supplied>
								try
								{
									writer.WriteString(" ");
									mezera.ZapsatMezeru = false;
								}
								catch (Exception e)
								{
									if (!e.Message.Contains("EndRootElement"))
										throw;
								}

							}
						}
					}
					//pro případy typu
					//<head><pb n="FS" rend="space"/><foreign>Levitici XI </foreign><note place="bottom">Lv 11,5–30</note></head>
					//mezera se nesmí přesunout za </head>
					//xxxx
					//if (mezera.Depth >= reader.Depth)
					//{
					//    //if(reader.Depth > 0)
					//    //    writer.WriteString(" ");
					//    mezera.ZapsatMezeru = false;
					//}
					//else if (vlozitMezeru)
					//{ //neexistují žádné uložené tagy, ale existuje mezera
					//	writer.WriteString(" ");
					//	vlozitMezeru = false;
					//}
					//if (vlozitMezeru)
					//	vlozitMezeru = false;
				}
				else
				{
					SerializeNode(reader, writer);
				}
			}
		}

		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="tagyPredNimizSeMezeraNepresouva">Seznam tagů, před nimiž se mezera nepřesouvá (např. hi, supplied).</param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		public static void PresunoutMezeryVneTagu2(XmlReader reader, XmlWriter writer,
			List<string> tagyPredNimizSeMezeraNepresouva,
			List<string> tagyKdeSeMezeraPresouvaZaZnacku)
		{
			PresunoutMezeryVneTagu2(reader, writer, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, String.Empty);
		}

		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="tagyPredNimizSeMezeraNepresouva">Seznam tagů, před nimiž se mezera nepřesouvá (např. hi, supplied).</param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		/// <param name="znakyInterpunkce">Seznam znaku, před nimiž se nevkládá mezera.</param>
		public static void PresunoutMezeryVneTagu2(XmlReader reader,
			XmlWriter writer,
			List<string> tagyPredNimizSeMezeraNepresouva,
			List<string> tagyKdeSeMezeraPresouvaZaZnacku,
			string znakyInterpunkce)
		{
			PresunoutMezeryVneTagu2(reader, writer, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, new List<string>(), znakyInterpunkce);
		}

		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="tagyPredNimizSeMezeraNepresouva">Seznam tagů, před nimiž se mezera nepřesouvá (např. hi, supplied).</param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		/// <param name="tagyKdeSePresouvaniVynechava"></param>
		/// <param name="znakyInterpunkce">Seznam znaku, před nimiž se nevkládá mezera.</param>
		public static void PresunoutMezeryVneTagu2(XmlReader reader,
			XmlWriter writer,
			List<string> tagyPredNimizSeMezeraNepresouva,
			List<string> tagyKdeSeMezeraPresouvaZaZnacku,
			List<string> tagyKdeSePresouvaniVynechava,
			string znakyInterpunkce)
		{
			PresunoutMezeryVneTagu2(reader, writer, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, tagyKdeSePresouvaniVynechava, new List<string>(), znakyInterpunkce);
		}

		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		/// <param name="tagyPredNimizSeMezeraNepresouva">Seznam tagů, před nimiž se mezera nepřesouvá (např. hi, supplied).</param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		/// <param name="tagyKdeSePresouvaniVynechava"></param>
		/// <param name="tagyKdeSePresouvaniIgnoruje">Seznam tagů, u nichž se kopíruje celý jejich obsah, aniž se zjišťuje, co je uvnitř. Obvykle se tagy uvedou i mezi <paramref name="tagyPredNimizSeMezeraNepresouva"/> Např. ins, del.</param>
		/// <param name="znakyInterpunkce">Seznam znaku, před nimiž se nevkládá mezera.</param>
		public static void PresunoutMezeryVneTagu2(XmlReader reader, XmlWriter writer, List<string> tagyPredNimizSeMezeraNepresouva, List<string> tagyKdeSeMezeraPresouvaZaZnacku, List<string> tagyKdeSePresouvaniVynechava, List<string> tagyKdeSePresouvaniIgnoruje, string znakyInterpunkce)
		{
			char[] interpunkce = znakyInterpunkce.ToCharArray();
			string currentTagName = null;

			Stack<XmlElementInfo> xmlElementInfos = new Stack<XmlElementInfo>();

			DocumentSpace spacesInDocument = new DocumentSpace();

			//Queue<string> qsTexty = new Queue<string>();
			if (tagyPredNimizSeMezeraNepresouva == null)
				tagyPredNimizSeMezeraNepresouva = new List<string>();
			if (tagyKdeSeMezeraPresouvaZaZnacku == null)
				tagyKdeSeMezeraPresouvaZaZnacku = new List<string>();
			if (tagyKdeSePresouvaniVynechava == null)
				tagyKdeSePresouvaniVynechava = new List<string>();
			if(tagyKdeSePresouvaniIgnoruje == null)
				tagyKdeSePresouvaniIgnoruje = new List<string>();

			const string textElementName = "#TEXT";

			int depth;

			while (reader.Read())
			{
			Zacatek:
				depth = reader.Depth;
				string readerName = reader.Name;
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:

						XmlElementInfo currentXmlElementInfo = new XmlElementInfo(readerName, depth, reader.IsEmptyElement, reader.NamespaceURI);

						if (tagyPredNimizSeMezeraNepresouva.Contains(readerName) && tagyKdeSePresouvaniIgnoruje.Contains(readerName))
						{
							if (ContainsSpace(spacesInDocument, depth - 1, depth))
								writer.WriteString(" ");
							InvalidatePreviousSpaces(spacesInDocument, depth - 1, depth);
						}

						if (tagyKdeSePresouvaniVynechava.Contains(readerName))
						{
							if (ContainsSpace(spacesInDocument, depth - 2, depth))
								writer.WriteString(" ");
							InvalidatePreviousSpaces(spacesInDocument, depth - 2, depth);
						}

						if (tagyKdeSePresouvaniIgnoruje.Contains(readerName))
						{
							SerializeWholeNode(reader, writer, true);
							reader.Read();
							goto  Zacatek;
						}



						if (reader.AttributeCount > 0)
						{

							if (reader.GetAttribute("rend") == "space")
							{
								//Space space = new Space(depth, true, currentTagName);
								Space space = new Space(true, currentXmlElementInfo);

								if (spacesInDocument.ContainsKey(depth))
								{
									if (spacesInDocument[depth].EndsWithSpace && !spacesInDocument[depth].SpaceAsAttribute) // když jsou v texut 2 @rend="space" za sebou (tj. bez jiných mezer)
										writer.WriteString(" ");
									//goto Konec;
									spacesInDocument.Remove(depth);
								}

								if (!spacesInDocument.ContainsKey(depth))
									spacesInDocument.Append(depth, space);
								else //přednost mají mezery vyjádřené textem, ne jako atribut (<note place="bottom">zapsáno „a“ </note><supplied>… </supplied><pb n="176r" rend="space"/>)
								{
									if (!spacesInDocument[depth].EndsWithSpace)
										spacesInDocument.Append(depth, space);
								}
								goto Serializace;
							}

						}
						if (tagyKdeSeMezeraPresouvaZaZnacku.Contains(currentXmlElementInfo.Name))
						{
							//goto Konec;
							goto Serializace;
							SerializeNode(reader, writer);
						}
						if (tagyPredNimizSeMezeraNepresouva.Contains(readerName))
						{
							for (int i = depth; i >= depth - 1; i--)
							{
								if (spacesInDocument.ContainsKey(i))
								{
									if (spacesInDocument[i].EndsWithSpace)
									{
										//if (!spacesInDocument[i].SpaceAsAttribute)
										if (spacesInDocument[i].SpaceAsAttribute)
										{
											if (xmlElementInfos.Peek().Name == textElementName)
												writer.WriteString(" ");
											else
												continue;
										}
										else
											writer.WriteString(" ");
										//goto Konec;
										spacesInDocument.Remove(i);
										//pro případy kombinace <note><note><add>
										if (i == depth)
										{
											if (spacesInDocument.ContainsKey(depth - 1))
												spacesInDocument.Remove(depth - 1);

										}
										goto Serializace;
									}

								}
							}
							/*
							if (spacesInDocument.ContainsKey(depth))
							{
								if (spacesInDocument[depth].EndsWithSpace)
									writer.WriteString(" ");
								//goto Konec;
								spacesInDocument.Remove(depth);
								goto Serializace;

							}
							else if (spacesInDocument.ContainsKey(depth - 1))
							{
								if (spacesInDocument[depth - 1].EndsWithSpace)
									writer.WriteString(" ");
								//goto Konec;
								spacesInDocument.Remove(depth - 1);
								goto Serializace;

							}
								*/
							//else
							{
								goto Konec;
								SerializeNode(reader, writer);
							}
						}
						else
						{
							goto Konec;
							SerializeNode(reader, writer);
						}
					Konec:
						if (spacesInDocument.ContainsKey(depth))
						{
							//if(spacesInDocument[depth] != null && spacesInDocument[depth].Text != null)
							//	writer.WriteString(spacesInDocument[depth].TextWithoutTrailingSpace);
							//if (spacesInDocument[depth] != null && spacesInDocument[depth].EndsWithSpace)
							//	writer.WriteString(" ");
						}
					Serializace:
						if (currentXmlElementInfo.IsElementEmpty)
						{
							if (spacesInDocument.ContainsKey(depth - 1))
							{
								if (spacesInDocument[depth - 1].EndsWithSpace)
									writer.WriteString(" ");
								//goto Konec;
								spacesInDocument.Remove(depth - 1);
							}
						}
						SerializeNode(reader, writer);
						if (!currentXmlElementInfo.IsElementEmpty)
						{
							//tagyDokumentu.Push(currentXmlElementInfo.Name);
							xmlElementInfos.Push(currentXmlElementInfo);
						}
						break;
					case XmlNodeType.SignificantWhitespace:
					case XmlNodeType.Text:
						{
							XmlElementInfo currentElementInfo = xmlElementInfos.Pop();
							currentElementInfo.ContainsText = true;
							xmlElementInfos.Push(currentElementInfo);

							string content = reader.Value;
							XmlElementInfo textElementInfo = new XmlElementInfo(textElementName, currentElementInfo.Depth, false, null);

							int elementDepth = currentElementInfo.Depth; // depth - 1;
							//Space actualSpace = new Space(elementDepth, reader.Value, currentTagName);
							//Space actualSpace = new Space(currentElementInfo.Depth, reader.Value, currentElementInfo.Name);
							Space actualSpace = new Space(content, currentElementInfo);


							Space previousSpace = null;
							if (spacesInDocument.ContainsKey(elementDepth + 1))
							{
								previousSpace = spacesInDocument[elementDepth + 1];
							}

							if (previousSpace == null || (!previousSpace.EndsWithSpace || previousSpace.SpaceAsAttribute))
								if (spacesInDocument.ContainsKey(elementDepth))
								{
									previousSpace = spacesInDocument[elementDepth];
								}

							if (!tagyKdeSeMezeraPresouvaZaZnacku.Contains(currentElementInfo.Name) || (previousSpace != null && currentElementInfo.Name == previousSpace.CurrentElementInfo.Name && currentElementInfo.Depth == previousSpace.CurrentElementInfo.Depth) )
							{
								if (previousSpace != null)
									if (previousSpace.EndsWithSpace)
									{
										if (!previousSpace.SpaceAsAttribute)
										{
											previousSpace.InvalidateTrailingSpace();
											if (!ZacinaNaInterpunkci(actualSpace.Text, interpunkce) && !actualSpace.Text.StartsWith(" "))
												writer.WriteString(" ");
										}
									}
							}

							/*
							if (previousSpace != null && previousSpace.EndsWithSpace && !actualSpace.EndsWithSpace && previousSpace.Depth == actualSpace.Depth)
							{
								actualSpace.SetTrailingSpace(true);
							}
							 */
							if (previousSpace != null && previousSpace.Depth == elementDepth && tagyKdeSeMezeraPresouvaZaZnacku.Contains(previousSpace.CurrentElementInfo.Name))
							{
								if (!previousSpace.EndsWithSpace && actualSpace.EndsWithSpace)
								{
									spacesInDocument.Append(elementDepth, actualSpace);
								}
							}
							else
								spacesInDocument.Append(elementDepth, actualSpace);

							writer.WriteString(actualSpace.TextWithoutTrailingSpace);

							/*
							if (!spacesInDocument.ContainsKey(elementDepth))
								spacesInDocument.Add(elementDepth, actualSpace);
							else
								spacesInDocument[elementDepth] = actualSpace;
							*/

						}
						break;
					case XmlNodeType.EndElement:
						XmlElementInfo endElementInfo = new XmlElementInfo(readerName, depth, false, reader.NamespaceURI);
						XmlElementInfo peekElementInfo = xmlElementInfos.Peek();
						if (peekElementInfo.Name == textElementName)
						{
							xmlElementInfos.Pop();
							peekElementInfo = xmlElementInfos.Peek();
						}

						if (endElementInfo.ToString() != peekElementInfo.ToString())
						{
							throw new Exception("Tagy si neodpovídají: " + peekElementInfo + " × " + endElementInfo);
						}
						/*
						if (tagyKdeSePresouvaniVynechava.Contains(readerName) && spacesInDocument.ContainsKey(depth))
						{
							if (spacesInDocument[depth].CurrentTag == readerName && spacesInDocument[depth].EndsWithSpace)
							{
								writer.WriteString(" ");
								spacesInDocument[depth].InvalidateTrailingSpace();
							}
						}
						*/

						SerializeNode(reader, writer);
						xmlElementInfos.Pop();

						/*					if (tagyKdeSeMezeraPresouvaZaZnacku.Contains(endElementInfo.Name))
											{
												if (spacesInDocument.ContainsKey(endElementInfo.Depth) && spacesInDocument[endElementInfo.Depth].CurrentElementInfo.ToString() == endElementInfo.ToString())
												{
													spacesInDocument.Remove(endElementInfo.Depth);
												}
											}
					*/
						RemoveUnusedKeys(endElementInfo, spacesInDocument, xmlElementInfos, tagyKdeSePresouvaniVynechava);

						/*
						if (tagyDokumentu.Count > 0)
						{
							string tag = tagyDokumentu.Peek();
							if (tag != readerName)
							{
								throw new Exception("Tagy si neodpovídají: " + tag + " × " + currentTagName);
							}
							SerializeNode(reader, writer);
							tagyDokumentu.Pop();
							xmlElementInfos.Pop();
							//RemoveUnusedKeys(depth, spacesInDocument);
							RemoveUnusedKeys(endElementInfo, spacesInDocument, xmlElementInfos);

						}*/
						break;
					case XmlNodeType.XmlDeclaration:
						SerializeNode(reader, writer);
						writer.WriteComment("PresunoutMezeryVneTagu");
						break;
					default:
						SerializeNode(reader, writer);
						break;
				}
			}

		}

		private static bool ContainsSpace(DocumentSpace spacesInDocument, int from, int to)
		{
			for (int depth = from; depth <= to; depth++)
			{
				if (spacesInDocument.ContainsKey(depth))
				{
					if(spacesInDocument[depth].EndsWithSpace) return true;
				}
			}
			return false;
		}

		private static void InvalidatePreviousSpaces(DocumentSpace spacesInDocument, int from, int to)
		{
			for (int depth = from; depth <= to; depth++)
			{
				if (spacesInDocument.ContainsKey(depth)) { 
					spacesInDocument[depth].InvalidateTrailingSpace();
				}
			}
		}

		private static bool ZacinaNaInterpunkci(string text, char[] interpunkce)
		{
			if (interpunkce == null || interpunkce.Length == 0) return false;
			if (String.IsNullOrEmpty(text)) return false;
			List<char> chrs = new List<char>(interpunkce);
			for (int i = 0; i < interpunkce.Length; i++)
			{
				if (interpunkce[i] == text[0]) return true;
			}
			return false;
		}

		private static void RemoveUnusedKeys(int depth, DocumentSpace spacesInDocument)
		{
			List<int> keysToRemove = new List<int>(depth);
			foreach (KeyValuePair<int, Space> kvp in spacesInDocument)
			{
				if (kvp.Key > depth)
				{
					keysToRemove.Add(kvp.Key);
				}
			}
			foreach (int key in keysToRemove)
			{
				spacesInDocument.Remove(key);
			}
		}

		private static void RemoveUnusedKeys(XmlElementInfo endElementInfo, 
			DocumentSpace spacesInDocument, 
			Stack<XmlElementInfo> xmlElementInfos,
			List<string> tagyKdeSePresouvaniVynechava)
		{
			List<int> keysToRemove = new List<int>();
			Dictionary<int, Space> keysToAdd = new Dictionary<int, Space>();
			//List<int> keysToAdd = new List<int>();
			Space spaceToAdd = null;
			foreach (KeyValuePair<int, Space> kvp in spacesInDocument)
			{
				if (kvp.Key > endElementInfo.Depth)
				{
					keysToRemove.Add(kvp.Key);
					if (kvp.Value.EndsWithSpace)
					{
						if (spacesInDocument.ContainsKey(kvp.Key - 1) &&
							!spacesInDocument[kvp.Key - 1].EndsWithSpace && 
							spacesInDocument[kvp.Key - 1].CurrentElementInfo.ToString() != endElementInfo.ToString())
						{
							spacesInDocument[kvp.Key - 1].ValidateTrailingSpace();
						}
						/*
						if (!spacesInDocument.ContainsKey(kvp.Key - 1) && (endElementInfo.Depth == (kvp.Key - 1)))
						{
							XmlElementInfo xmlElementInfo = endElementInfo.Clone();
							if (!xmlElementInfo.ContainsText)
							{
								spaceToAdd = kvp.Value.Clone();
								spaceToAdd.CurrentElementInfo = xmlElementInfo;
								spaceToAdd.Text = " ";
								spaceToAdd.Depth = xmlElementInfo.Depth;

								keysToAdd.Add(kvp.Key - 1, spaceToAdd);

							}
						}
						 */
					}
				}
			}
			Space currentSpace = spacesInDocument.ContainsKey(endElementInfo.Depth) ? spacesInDocument[endElementInfo.Depth] : null;
			if (keysToRemove.Count > 0)
			{
				Space deeperSpace = spacesInDocument.ContainsKey(endElementInfo.Depth + 1) ? spacesInDocument[endElementInfo.Depth + 1] : null;

				if (currentSpace != null
				    && (!currentSpace.EndsWithSpace
				        || currentSpace.CurrentElementInfo.ToString() == endElementInfo.ToString())
					&& (deeperSpace != null && (!deeperSpace.EndsWithSpace || !deeperSpace.SpaceInvalidated)
					 ))
				{
					if (tagyKdeSePresouvaniVynechava.Contains(endElementInfo.Name))
					{
						if (
							
							(deeperSpace.EndsWithSpace || deeperSpace.SpaceInvalidated)
							)
							currentSpace.SetTrailingSpace(true);
					}
					else
					{ 
						if((!(deeperSpace.EndsWithSpace || deeperSpace.SpaceInvalidated) || deeperSpace.CurrentElementInfo.IsElementEmpty) &&
						currentSpace.EndsWithSpace /*&& !spacesInDocument[endElementInfo.Depth + 1].SpaceAsAttribute*/)
						keysToRemove.Add(endElementInfo.Depth); 
					}
				}
			}
			if (keysToRemove.Count == 0 && xmlElementInfos.Count > 0)
			{
				XmlElementInfo previousElementInfo = xmlElementInfos.Peek();
				if (/* endElementInfo.Depth >= 2 &&
					xmlElementInfos.Count > 1 &&  */
					previousElementInfo.Depth + 1 >= endElementInfo.Depth &&
					(!spacesInDocument.ContainsKey(endElementInfo.Depth - 1) || ((endElementInfo.Depth == previousElementInfo.Depth + 1) && !previousElementInfo.ContainsText))
					/* &&
					!previousElementInfo.ContainsText */)
				{
					if (currentSpace != null && currentSpace.EndsWithSpace)
					{
						XmlElementInfo xmlElementInfo = xmlElementInfos.Peek().Clone();
						spaceToAdd = currentSpace.Clone();
						spaceToAdd.CurrentElementInfo = xmlElementInfo;
						spaceToAdd.Text = " ";
						spaceToAdd.Depth = xmlElementInfo.Depth;

						keysToAdd.Add(xmlElementInfo.Depth, spaceToAdd);
						keysToRemove.Add(endElementInfo.Depth);
					}
				}
			}
			foreach (int key in keysToRemove)
			{
				if (spacesInDocument.ContainsKey(key))
					spacesInDocument.Remove(key);
			}

			foreach (KeyValuePair<int, Space> kvp in keysToAdd)
			{
				spacesInDocument.Append(kvp.Key, kvp.Value);
			}

		}

		/// <summary>
		/// Přesune koncovou mezeru z textového uzlu za nejbližší značku.
		/// </summary>
		/// <param name="reader">XmlReader s odkazem na aktuální uzel, který se má zpracovat.</param>
		/// <param name="writer">XmlWriter nového dokumentu, do něhož se má uzel překopírovat.</param>
		public static void PresunoutMezeryVneTagu(XmlReader reader, XmlWriter writer)
		{
			PresunoutMezeryVneTagu(reader, writer, null, null);
		}

		public static void PresunoutMezeryVneTagu(string strVstup, string strVystup)
		{
			XmlReaderSettings xrs = new XmlReaderSettings();
			PresunoutMezeryVneTagu(strVstup, strVystup, xrs);
		}

		public static void PresunoutMezeryVneTagu(string strVstup, string strVystup, XmlReaderSettings xmlReaderSettings)
		{
			PresunoutMezeryVneTagu(strVstup, strVystup, xmlReaderSettings, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vstup"></param>
		/// <param name="vystup"></param>
		/// <param name="xmlReaderSettings"></param>
		/// <param name="tagyPredNimizSeMezeraNepresouva"></param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		public static void PresunoutMezeryVneTagu(string vstup, string vystup, XmlReaderSettings xmlReaderSettings,
			List<string> tagyPredNimizSeMezeraNepresouva, List<string> tagyKdeSeMezeraPresouvaZaZnacku)
		{
			PresunoutMezeryVneTagu(vstup, vystup, xmlReaderSettings, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, ZnakyInterpunkce);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vstup"></param>
		/// <param name="vystup"></param>
		/// <param name="xmlReaderSettings"></param>
		/// <param name="tagyPredNimizSeMezeraNepresouva"></param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		/// <param name="znakyInterpunkce">Seznam znaku, před nimiž se nevkládá mezera.</param>
		public static void PresunoutMezeryVneTagu(string vstup, string vystup,
			XmlReaderSettings xmlReaderSettings,
			List<string> tagyPredNimizSeMezeraNepresouva,
			List<string> tagyKdeSeMezeraPresouvaZaZnacku, string znakyInterpunkce)
		{
			PresunoutMezeryVneTagu(vstup, vystup, xmlReaderSettings, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, new List<string>(), znakyInterpunkce);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vstup"></param>
		/// <param name="vystup"></param>
		/// <param name="xmlReaderSettings"></param>
		/// <param name="tagyPredNimizSeMezeraNepresouva"></param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		/// <param name="tagyKdeSePresouvaniVynechava"></param>
		/// <param name="znakyInterpunkce">Seznam znaku, před nimiž se nevkládá mezera.</param>
		public static void PresunoutMezeryVneTagu(
			string vstup,
			string vystup,
			XmlReaderSettings xmlReaderSettings,
			List<string> tagyPredNimizSeMezeraNepresouva,
			List<string> tagyKdeSeMezeraPresouvaZaZnacku,
			List<string> tagyKdeSePresouvaniVynechava, string znakyInterpunkce)
		{
			PresunoutMezeryVneTagu(vstup, vystup, xmlReaderSettings, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, tagyKdeSePresouvaniVynechava, new List<string>(), znakyInterpunkce);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vstup">Vstupní soubor.</param>
		/// <param name="vystup">Výstupní soubor.</param>
		/// <param name="xmlReaderSettings">Nastavení <see cref="XmlReaderSettings"/> pro vstupní soubor.</param>
		/// <param name="tagyPredNimizSeMezeraNepresouva"></param>
		/// <param name="tagyKdeSeMezeraPresouvaZaZnacku">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
		/// <param name="tagyKdeSePresouvaniVynechava"></param>
		/// <param name="tagyKdeSePresouvaniIgnoruje">Seznam tagů, u nichž se kopíruje celý jejich obsah, aniž se zjišťuje, co je uvnitř. Obvykle se tagy uvedou i mezi <paramref name="tagyPredNimizSeMezeraNepresouva"/> Např. ins, del.</param>
		/// <param name="znakyInterpunkce">Seznam znaku, před nimiž se nevkládá mezera.</param>
		public static void PresunoutMezeryVneTagu(string vstup, string vystup, 
			XmlReaderSettings xmlReaderSettings,
			List<string> tagyPredNimizSeMezeraNepresouva, 
			List<string> tagyKdeSeMezeraPresouvaZaZnacku, 
			List<string> tagyKdeSePresouvaniVynechava, 
			List<string> tagyKdeSePresouvaniIgnoruje, 
			string znakyInterpunkce)
		{
			Console.WriteLine("{0} => {1}, Přesunout mezery vně tagu", vstup, vystup);
			using (XmlReader xr = XmlReader.Create(vstup, xmlReaderSettings))
			{
				using (XmlWriter xw = XmlWriter.Create(vystup))
				{
					// XXXX
					//PresunoutMezeryVneTagu(xr, xw, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku);
					PresunoutMezeryVneTagu2(xr, xw, tagyPredNimizSeMezeraNepresouva, tagyKdeSeMezeraPresouvaZaZnacku, tagyKdeSePresouvaniVynechava, tagyKdeSePresouvaniIgnoruje, znakyInterpunkce);

				}
			}
		}

		public static void PresunoutMezeryVneTagu(string strVstup, string strVystup, XmlReaderSettings xmlReaderSettings,
			List<string> tagyPredNimizSeMezeraNepresouva)
		{
			PresunoutMezeryVneTagu(strVstup, strVystup, xmlReaderSettings, tagyPredNimizSeMezeraNepresouva, null);
		}



		#endregion

	}
}
