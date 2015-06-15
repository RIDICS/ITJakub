using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.XmlDiffPatch;
using System.IO;

namespace Daliboris.Pomucky.Xml
{
    public class Porovnani
    {
        private XmlDocument mxdLevy;
        private XmlDocument mxdPravy;

        public Porovnani(XmlDocument xdLevy, XmlDocument xdPravy)
        {
            mxdLevy = xdLevy;
            mxdPravy = xdPravy;
        }
        public XmlDocument Porovnej()
        {
            XmlDocument xdd = null;
            string sTemp = Path.GetTempFileName();
            XmlWriter xw = new XmlTextWriter(sTemp, Encoding.UTF8);
            XmlDiff xd = new XmlDiff();
            if (xd.Compare(mxdLevy.DocumentElement, mxdPravy.DocumentElement, xw))
            {
                xdd = new XmlDocument();
                xdd.Load(sTemp);
            }
            return xdd;
        }
        public static XmlDocument Porovnej(XmlDocument xdLevy, XmlDocument xdPravy)
        {
            XmlDocument xdd = null;

            try
            {
                XmlDiffOptions options = XmlDiffOptions.None;
                MemoryStream diffgram = new MemoryStream();
                XmlTextWriter diffgramWriter = new XmlTextWriter(new StreamWriter(diffgram));
                XmlDiff xmlDiff = new XmlDiff(options);
                //bool bIdentical = xmlDiff.Compare(@"V:\Temp\Evidence\1.xml", @"V:\Temp\Evidence\12.xml", false, diffgramWriter);
                bool bIdentical = xmlDiff.Compare(xdLevy.DocumentElement, xdPravy.DocumentElement, diffgramWriter);
                if (!bIdentical)
                {
                    diffgram.Seek(0, SeekOrigin.Begin);
                    xdd = new XmlDocument();
                    xdd.Load(new XmlTextReader(diffgram));
                    xdd.Save(@"V:\Temp\Evidence\diff.xml");
                }

            }
            catch
            {
                return null;
            }
            /*
            XmlDiff xd = new XmlDiff();
            xd.Algorithm = XmlDiffAlgorithm.Precise;
            xd.IgnoreWhitespace = true;
            if (xd.Compare(@"V:\Temp\Evidence\1.xml", @"V:\Temp\Evidence\12.xml", true, xw))
             **/
            return xdd;
        }
    }
}
