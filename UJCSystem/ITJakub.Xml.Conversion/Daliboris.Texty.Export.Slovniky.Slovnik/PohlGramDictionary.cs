using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Daliboris.Pomucky.Xml;
using DPXT = Daliboris.Pomucky.Xml.Transformace;
using Ujc.Ovj.Xml.Info;

namespace Daliboris.Slovniky
{
    public class PohlGramDictionary : MockDictionary
    {
        public override void KonsolidovatHeslovouStat(string inputFile, string outputFile)
        {
            int iEntry = 0;
            string sSource = null;
            using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
            {
                using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
                {
                    xw.WriteStartDocument(true);


                    while (r.Read())
                    {
                        if (r.NodeType == XmlNodeType.Element)
                        {
                            switch (r.Name)
                            {
                                case "entry":
                                    XmlDocument xd = new XmlDocument();
                                    XmlNode xn = xd.ReadNode(r);
                                    if (xn != null)
                                        xd.AppendChild(xn);
                                    if (xd.DocumentElement != null)
                                        if (!xd.DocumentElement.IsEmpty)
                                        {
                                            ZkonsolidujEntry(ref xd, sSource, ++iEntry);
                                            xd.WriteContentTo(xw);
                                        }

                                    break;
                                case "dictionary":
                                    sSource = r.GetAttribute("name");
                                    goto default;
                                default:
                                    Transformace.SerializeNode(r, xw);
                                    break;

                            }
                        }
                        else if (r.NodeType == XmlNodeType.EndElement)
                        {
                            switch (r.Name)
                            {
                                case "entry":
                                    break;
                                default:
                                    Transformace.SerializeNode(r, xw);
                                    break;
                            }
                        }
                        else { Transformace.SerializeNode(r, xw); }

                    }

                }
            }
        }

        private void ZkonsolidujEntry(ref XmlDocument xd, string sSlovnikID, int iEntry)
        {

            if (xd.DocumentElement == null)
                return;
            //Přiřadí heslové stati identifikátor
            XmlAttribute xa = xd.CreateAttribute("id");
            xa.Value = "en" + iEntry.ToString("000000");
            xd.DocumentElement.Attributes.Append(xa);

            //Přiřadí heslové stati zdroj (z jakého slovníku pochází)
            xa = xd.CreateAttribute("source");
            xa.Value = sSlovnikID;
            xd.DocumentElement.Attributes.Append(xa);

            //Zjišťuje typ heslové stati (uvedený v atributu type u značky entryhead)

            XmlNode xn = xd.SelectSingleNode("//entryhead");
            if (xn != null)
            {
                //Pokud existuje atibut type existuje, jeho hodnota se přiřadí ke značce entry
                if (xn.Attributes.Count == 1)
                {
                    xa = (XmlAttribute)xn.Attributes[0].Clone();
                    xd.DocumentElement.Attributes.Append(xa);
                    //nakonec se atribut type u značky entryhead odstraní (byl tam jenom kvůli generování z Wordu)
                    xn.Attributes.Remove(xn.Attributes[0]);
                }
                else
                {
                    //pokud entryhead atribut nemá, jde o plnohodnotnou heslovou stať
                    xa = xd.CreateAttribute("type");
                    xa.Value = "full";
                    xd.DocumentElement.Attributes.Append(xa);
                }
            }
            //heslové stati se přiřadí výchozí heslové slovo
            //TODO mělo by se dávat pozor na zkrácené podoby?
            xn = xd.SelectSingleNode("//hw");
            xa = xd.CreateAttribute("defaulthw");
            xa.Value = xn.InnerText.TrimEnd(Slovnik.CarkaTeckaMezeraStrednik).TrimEnd();
            xd.DocumentElement.Attributes.Append(xa);

            string sVychoziHeslo = xa.Value;
            xa = xd.CreateAttribute("defaulthwsort");
            xa.Value = Slovnik.HesloBezKrizkuAHvezdicky(sVychoziHeslo);
            xd.DocumentElement.Attributes.Append(xa);


            //každému heslovému slovu se přiřadí identifikátor
            XmlNodeList xnl = xd.SelectNodes("//hw");
            for (int i = 0; i < xnl.Count; i++)
            {
                string sIdHw = (i + 1).ToString();
                xa = xd.CreateAttribute("id");
                xa.Value = "en" + iEntry.ToString("000000") + ".hw" + sIdHw;
                xnl[i].Attributes.Append(xa);
                string sHeslo = xnl[i].InnerText;

                //identifikovat číslo homonyma
                int iHomonymum = sHeslo.IndexOfAny(Slovnik.IndexyCislic);
                if (iHomonymum > 0)
                {
                    string sHomonymum = sHeslo.Substring(iHomonymum, 1);
                    xa = xd.CreateAttribute("hom");
                    xa.Value = sHomonymum;
                    xnl[i].Attributes.Append(xa);
                }
            }
        }
    }
}
