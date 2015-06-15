using System;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

namespace Daliboris.Pomucky.Xml
{
    public static class Transformace
    {

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
                        w.WriteEndElement();
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

        private class VyclenenaMezera
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public VyclenenaMezera(int depth)
            {
                Depth = depth;
                ZapsatMezeru = true;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public VyclenenaMezera(int depth, string tag)
                : this(depth)
            {
                Tag = tag;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public VyclenenaMezera()
            {
            }

            /// <summary>
            /// Zanoření elementu ve struktuře XML
            /// </summary>
            public int Depth { get; set; }

            /// <summary>
            /// Zda se má zapsat mezera do výstupu (tj. předchozí textový element obsahoval mezeru a je potřeba jej přesunout
            /// </summary>
            public bool ZapsatMezeru { get; set; }

            /// <summary>
            /// Názel elementu, kde se mezera vyskytla
            /// </summary>
            public string Tag { get; set; }
        }

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
                    if(predchoziTagy.Count > 0)
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
        /// <param name="glsVynechaneTagy"></param>
        /// <param name="preskoceneTagy">Seznam výjimek, u nichž se mezera přesouvá až za tuto značku (např. note)</param>
        public static void PresunoutMezeryVneTagu(string vstup, string vystup, XmlReaderSettings xmlReaderSettings, List<string> glsVynechaneTagy, List<string> preskoceneTagy)
        {
            using (XmlReader xr = XmlReader.Create(vstup, xmlReaderSettings))
            {
                using (XmlWriter xw = XmlWriter.Create(vystup))
                {
                    PresunoutMezeryVneTagu(xr, xw, glsVynechaneTagy, preskoceneTagy);
                }
            }
        }

        public static void PresunoutMezeryVneTagu(string strVstup, string strVystup, XmlReaderSettings xmlReaderSettings, List<string> glsVynechaneTagy)
        {
            PresunoutMezeryVneTagu(strVstup, strVystup, xmlReaderSettings, glsVynechaneTagy, null);
        }

        public static void PridatMezeryZaTagyPoInterpunkci(string vstup, string vystup, XmlReaderSettings xmlReaderSettings, List<string> tagyPoInterpunkci, string znakyInterpunkce)
        {
            using (XmlReader xr = XmlReader.Create(vstup, xmlReaderSettings))
            {
                using (XmlWriter xw = XmlWriter.Create(vystup))
                {
                    PridatMezeryZaTagyPoInterpunkci(xr, xw, tagyPoInterpunkci, znakyInterpunkce);
                }
            }
        }

        private static void PridatMezeryZaTagyPoInterpunkci(XmlReader reader, XmlWriter writer, List<string> tagyPoInterpunkci, string znakyInterpunkce)
        {
            List<char> interpunkce = new List<char>(znakyInterpunkce.ToCharArray());

            Queue<string> qsTagy = new Queue<string>();
            bool vlozitMezeru = false;
            bool bylaInterpunkce = false;
            //Queue<string> qsTexty = new Queue<string>();


            string mstrAktualniText = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (bylaInterpunkce && !tagyPoInterpunkci.Contains(reader.Name))
                        vlozitMezeru = false;


                    //if (vlozitMezeru)
                    //{
                    //    if (reader.IsEmptyElement /* || vynechaneTagy.Contains(reader.Name) */ )
                    //    //u prázdných elementů se mezera za element neposouvá
                    //    {
                    //        writer.WriteString(" ");
                    //        vlozitMezeru = false;
                    //    }
                    //    else // nejede-li o prázdný element, zaznamená se jeho jméno do seznamu
                    //        qsTagy.Enqueue(reader.Name);
                    //    //if (qsTagy.Count == 1)
                    //    //    vlozitMezeru = false;
                    //}
                    SerializeNode(reader, writer);
                }
                else if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.SignificantWhitespace)
                {
                    mstrAktualniText = reader.Value;
                    //mstrAktualniText = mstrAktualniText.Replace(" ,", ",").Replace(" .", ".").Replace(" !", "!").Replace(" ?", "?");

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
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
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
                    //string sTag = reader.Name;
                    //SerializeNode(reader, writer);
                    //if (qsTagy.Count > 0)
                    //{
                    //    string sPrevTag = qsTagy.Peek();
                    //    if (sPrevTag == sTag)
                    //    {
                    //        //writer.WriteString(" ");
                    //        qsTagy.Dequeue();
                    //    }
                    //    else
                    //    {
                    //        if (vlozitMezeru)
                    //        { //mezera se vyskytla v rámci jednoho tagu <supplied>text </supplied>
                    //            try
                    //            {
                    //                writer.WriteString(" ");
                    //                vlozitMezeru = false;
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                if (!e.Message.Contains("EndRootElement"))
                    //                    throw;
                    //            }

                    //        }
                    //    }
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

    }
}
