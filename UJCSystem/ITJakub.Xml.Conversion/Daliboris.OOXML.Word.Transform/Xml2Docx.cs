using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Daliboris.OOXML.Word.Transform
{
    public class Xml2Docx
    {
        private string mstrSouborDocx;
        private string mstrSouborXml2Docx;
        private string mstrSouborXml;
        private Transformace mtrTransformace;

        //pokud se soubor změnil, je potřeba znovu nahrát transformace
        private bool mblnZmenaSouboruXml2Docx;
        private Dictionary<string, string> mgdcTag2OdstavcovyStyl;
        private Dictionary<string, string> mgdcTag2ZnakovyStyl;

        public Xml2Docx()
        {
        }

        public Xml2Docx(string strSouborXml, string strSouborDocx,
            Dictionary<string, string> gdcTag2OdstavcovyStyl, Dictionary<string, string> gdcTag2ZnakovyStyl)
            : this(strSouborXml, strSouborDocx)
        {
            mgdcTag2OdstavcovyStyl = gdcTag2OdstavcovyStyl;
            mgdcTag2ZnakovyStyl = gdcTag2ZnakovyStyl;

        }
        public Xml2Docx(string strSouborXml, string strSouborDocx, string strSouborXml2Docx)
            : this(strSouborDocx, strSouborXml)
        {
            mstrSouborDocx = strSouborDocx;
            mstrSouborXml2Docx = strSouborXml2Docx;
            mstrSouborXml = strSouborXml;
        }

        public Xml2Docx(string strSouborXml, string strSouborDocx)
            : this()
        {
            mstrSouborDocx = strSouborDocx;
            mstrSouborXml = strSouborXml;
        }

        public string SouborDocx
        {
            get { return mstrSouborDocx; }
            set { mstrSouborDocx = value; }
        }

        public string SouborXml2Docx
        {
            get { return mstrSouborXml2Docx; }
            set
            {
                if (mstrSouborXml2Docx != value)
                {
                    mblnZmenaSouboruXml2Docx = true;
                    mstrSouborXml2Docx = value;
                }
            }
        }

        public string SouborXml
        {
            get { return mstrSouborXml; }
            set { mstrSouborXml = value; }
        }

        public void Generate()
        {
            if (mtrTransformace == null)
            {
                mtrTransformace = new Transformace(mstrSouborXml2Docx);
                mtrTransformace.NactiZeSouboru();
                mblnZmenaSouboruXml2Docx = false;

            }
            else
            {
                if (mblnZmenaSouboruXml2Docx)
                {
                    mtrTransformace.Soubor = mstrSouborXml2Docx;
                    mtrTransformace.NactiZeSouboru();
                    mblnZmenaSouboruXml2Docx = false;
                }


            }
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = true;
            const string csNsW = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            const string csnNsR = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ProhibitDtd = false;

            string sText = null;
            using (XmlReader xr = XmlReader.Create(mstrSouborXml, xrs))
            {
                using (XmlWriter xw = XmlWriter.Create(mstrSouborDocx, xws))
                {

                    xw.WriteStartDocument();
                    xw.WriteStartElement("w", "document", csNsW);
                    xw.WriteAttributeString("xmlns", "r", null, csnNsR);
                    InsertManuscriptoriumProlog(xw);

                    while (xr.Read())
                    {
                        string sName = xr.Name;
                        sName = sName.Replace("_", ""); //identifikátory jsou bez podtržítek
                        if (xr.NodeType == XmlNodeType.Element && !xr.IsEmptyElement)
                        {
                            if (sName == "body") continue;

                            //if (mgdcTag2ZnakovyStyl.ContainsKey(sName)) {
                            //    //vytvořit run

                            //}

                            if (sName[0] == sName.ToLower()[0]) //jde o malé písmeno => odstavcový styl
                            {
                                //vytvořit run
                                xw.WriteStartElement("w", "r", csNsW);
                                xw.WriteStartElement("w", "rPr", csNsW);

                                xw.WriteStartElement("w", "rStyle", csNsW);
                                xw.WriteAttributeString("w", "val", csNsW, sName);
                                xw.WriteEndElement();
                                xw.WriteEndElement();

                                sText = xr.ReadString();
                                ZapsatWText(csNsW, sText, xw);

                            }
                            else if (sName[0] == sName.ToUpper()[0])
                            {
                                xw.WriteStartElement("w", "p", csNsW);
                                xw.WriteStartElement("w", "pPr", csNsW);
                                xw.WriteStartElement("w", "pStyle", csNsW);
                                xw.WriteAttributeString("w", "val", csNsW, sName);
                                xw.WriteEndElement();
                                xw.WriteEndElement();
                            }

                            #region původní kód

                            //else if (mgdcTag2OdstavcovyStyl.ContainsKey(sName)) {
                            //    xw.WriteStartElement("w", "p", csNsW);
                            //    xw.WriteStartElement("w", "pPr", csNsW);
                            //    xw.WriteStartElement("w", "pStyle", csNsW);
                            //    xw.WriteAttributeString("w", "val", csNsW, mgdcTag2OdstavcovyStyl[sName]);
                            //    xw.WriteEndElement();
                            //    xw.WriteEndElement();
                            //}
                            //else if (sName == "letter") {
                            //    xw.WriteStartElement("w", "p", csNsW);
                            //    xw.WriteStartElement("w", "pPr", csNsW);
                            //    xw.WriteStartElement("w", "pStyle", csNsW);
                            //    xw.WriteAttributeString("w", "val", csNsW, "Pismeno");
                            //    xw.WriteEndElement();
                            //    xw.WriteEndElement();

                            //    xw.WriteStartElement("w", "r", csNsW);
                            //    xw.WriteStartElement("w", "rPr", csNsW);
                            //    xw.WriteStartElement("w", "rStyle", csNsW);

                            //    xw.WriteAttributeString("w", "val", csNsW, "text");

                            //    xw.WriteEndElement();
                            //    xw.WriteEndElement();

                            //    sText = xr.GetAttribute("character");
                            //    ZapsatWText(csNsW, sText, xw);
                            //    xw.WriteEndElement();
                            //    xw.WriteEndElement();

                            //}

                            #endregion


                        }
                        if (xr.NodeType == XmlNodeType.EndElement && !xr.IsEmptyElement)
                        {
                            if (sName == "letter")
                            {
                                continue;
                            }
                            if (sName == "body")
                            {
                                continue;
                            }
                            else
                            {
                                xw.WriteEndElement();
                            }

                            //else if (mgdcTag2ZnakovyStyl.ContainsKey(sName) || mgdcTag2OdstavcovyStyl.ContainsKey(sName)) {
                            //    //ukončit run ne odstavec
                            //    xw.WriteEndElement();
                            //}
                        }

                        if (xr.NodeType == XmlNodeType.Text && !String.IsNullOrEmpty(xr.Value) && xr.Value[0] != 8233)
                        {
                            xw.WriteStartElement("w", "r", csNsW);
                            xw.WriteStartElement("w", "rPr", csNsW);

                            xw.WriteStartElement("w", "rStyle", csNsW);
                            xw.WriteAttributeString("w", "val", csNsW, "text");
                            xw.WriteEndElement();
                            xw.WriteEndElement();

                            ZapsatWText(csNsW, xr.Value, xw);
                            xw.WriteEndElement();//w:p - nejspíš konec odstavce
                        }
                    }
                    InsertManuscriptoriumEpilog(xw);
                    xw.WriteEndElement();//w:document
                    xw.WriteEndDocument();
                    xw.Flush();
                }
            }

        }

        private static void ZapsatWText(string csNsW, string sText, XmlWriter xw)
        {
            if (String.IsNullOrEmpty(sText)) return;
            if (Char.IsWhiteSpace(sText, 0) || Char.IsWhiteSpace(sText, sText.Length - 1))
            {
                xw.WriteStartElement("w", "t", csNsW);
                xw.WriteAttributeString("xml", "space", null, "preserve");
                xw.WriteString(sText);
                xw.WriteEndElement();
            }
            else
                xw.WriteElementString("w", "t", csNsW, sText);
        }

        private static void InsertManuscriptoriumProlog(XmlWriter xw)
        {
            //string data = "<w:document xmlns:wpc='http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas' xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:r='http://schemas.openxmlformats.org/officeDocument/2006/relationships' xmlns:m='http://schemas.openxmlformats.org/officeDocument/2006/math' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:wp14='http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing' xmlns:wp='http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing' xmlns:w10='urn:schemas-microsoft-com:office:word' xmlns:w='http://schemas.openxmlformats.org/wordprocessingml/2006/main' xmlns:w14='http://schemas.microsoft.com/office/word/2010/wordml' xmlns:wpg='http://schemas.microsoft.com/office/word/2010/wordprocessingGroup' xmlns:wpi='http://schemas.microsoft.com/office/word/2010/wordprocessingInk' xmlns:wne='http://schemas.microsoft.com/office/word/2006/wordml' xmlns:wps='http://schemas.microsoft.com/office/word/2010/wordprocessingShape' mc:Ignorable='w14 wp14'><w:body><w:tbl><w:tblPr><w:tblW w:w='9767' w:type='dxa'/><w:tblBorders><w:top w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:left w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:insideH w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:insideV w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tblBorders><w:tblLook w:val='0080' w:firstRow='0' w:lastRow='0' w:firstColumn='1' w:lastColumn='0' w:noHBand='0' w:noVBand='0'/></w:tblPr><w:tblGrid><w:gridCol w:w='2103'/><w:gridCol w:w='7664'/></w:tblGrid><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:bookmarkStart w:id='0' w:name='_GoBack'/><w:bookmarkEnd w:id='0'/><w:r><w:rPr><w:b/></w:rPr><w:t>Autor</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Titul</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Datace pramene</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Tiskař</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Místo</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='FF6600'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:jc w:val='center'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Předloha přepisu</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Typ</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='nil'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='99CC00'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Uložení rkp/tisku</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='nil'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='99CC00'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Země</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Město</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Instituce</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Signatura</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Foliace/paginace</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='99CC00'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Edice</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Titul</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Místo vydání</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Rok vydání</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Strany</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='FF6600'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:jc w:val='center'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Přepis</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (1)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (2)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (3)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (4)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='FF6600'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:jc w:val='center'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Poznámky</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='double' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr></w:tbl>";
            string data = "<w:body><w:tbl><w:tblPr><w:tblW w:w='9767' w:type='dxa'/><w:tblBorders><w:top w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:left w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:insideH w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:insideV w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tblBorders><w:tblLook w:val='0080' w:firstRow='0' w:lastRow='0' w:firstColumn='1' w:lastColumn='0' w:noHBand='0' w:noVBand='0'/></w:tblPr><w:tblGrid><w:gridCol w:w='2103'/><w:gridCol w:w='7664'/></w:tblGrid><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:bookmarkStart w:id='0' w:name='_GoBack'/><w:bookmarkEnd w:id='0'/><w:r><w:rPr><w:b/></w:rPr><w:t>Autor</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Titul</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Datace pramene</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Tiskař</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Místo</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='FF6600'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:jc w:val='center'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Předloha přepisu</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Typ</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='nil'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='99CC00'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Uložení rkp/tisku</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='nil'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='99CC00'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Země</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Město</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Instituce</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Signatura</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Foliace/paginace</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='99CC00'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Edice</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Titul</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Místo vydání</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Rok vydání</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Strany</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='FF6600'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:jc w:val='center'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Přepis</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (1)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (2)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (3)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='2103' w:type='dxa'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:right w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='CCFFFF'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:rPr><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Editor (4)</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w='7664' w:type='dxa'/><w:tcBorders><w:left w:val='single' w:sz='18' w:space='0' w:color='auto'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='double' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='single' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='FF6600'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/><w:jc w:val='center'/></w:pPr><w:r><w:rPr><w:b/></w:rPr><w:t>Poznámky</w:t></w:r></w:p></w:tc></w:tr><w:tr><w:tc><w:tcPr><w:tcW w:w='9767' w:type='dxa'/><w:gridSpan w:val='2'/><w:tcBorders><w:top w:val='single' w:sz='6' w:space='0' w:color='000000'/><w:bottom w:val='double' w:sz='6' w:space='0' w:color='000000'/></w:tcBorders><w:shd w:val='clear' w:color='auto' w:fill='auto'/></w:tcPr><w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p></w:tc></w:tr></w:tbl>";
            xw.WriteRaw(data);
            data = "<w:p><w:pPr><w:pStyle w:val='Hlavicka'/></w:pPr></w:p>";
            xw.WriteRaw(data);
        }

        private static void InsertManuscriptoriumEpilog(XmlWriter xw)
        {
            //string data = "<w:sectPr><w:headerReference w:type='default' r:id='rId8'/><w:pgSz w:w='11906' w:h='16838'/><w:pgMar w:top='1134' w:right='1134' w:bottom='1134' w:left='1134' w:header='709' w:footer='709' w:gutter='0'/><w:cols w:space='708'/><w:formProt w:val='0'/><w:docGrid w:linePitch='360'/></w:sectPr></w:body></w:document>";
            string data = "<w:sectPr><w:headerReference w:type='default' r:id='rId8'/><w:pgSz w:w='11906' w:h='16838'/><w:pgMar w:top='1134' w:right='1134' w:bottom='1134' w:left='1134' w:header='709' w:footer='709' w:gutter='0'/><w:cols w:space='708'/><w:formProt w:val='0'/><w:docGrid w:linePitch='360'/></w:sectPr></w:body>";
            xw.WriteRaw(data);
        }



    }
}
