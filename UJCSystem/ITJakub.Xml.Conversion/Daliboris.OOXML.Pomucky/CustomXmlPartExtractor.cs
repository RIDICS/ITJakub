using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Daliboris.OOXML.Pomucky
{
    public class CustomXmlPartExtractor
    {
        public CustomXmlPartExtractor(object settings)
        {
            Settings = settings;
        }

        public CustomXmlPartExtractorSettings TypedSetting {
            get { return Settings as CustomXmlPartExtractorSettings; }
        }

        public object Settings { get; set; }

        public void Extract()
        {
            CheckIfTypedSettingIsNull();

            using (WordprocessingDocument document = WordprocessingDocument.Open(TypedSetting.DocxFilePath, true))
            {

                string dataStoreItemId = TypedSetting.DataStoreItemId;
                CustomXmlPart xmlPart = GetCustomXmlPart(document, dataStoreItemId);

                if (xmlPart != null)
                {
                    using (StreamReader stream = new StreamReader(xmlPart.GetStream(FileMode.Open, FileAccess.Read)))
                    {
                        XDocument doc = XDocument.Load(stream);
                        doc.Save(TypedSetting.CustomXmlFilePath);
                    }
                }
            }
        }

        private void CheckIfTypedSettingIsNull()
        {
            if (TypedSetting == null) throw new NullReferenceException();
        }

        private static CustomXmlPart GetCustomXmlPart(WordprocessingDocument document, string dataStoreItemId)
        {
            CustomXmlPart xmlPart = null;
            List<CustomXmlPart> xmlParts = document.MainDocumentPart.CustomXmlParts.ToList();


            foreach (CustomXmlPart customXmlPart in xmlParts)
            {
                if (customXmlPart.CustomXmlPropertiesPart != null &&
                    customXmlPart.CustomXmlPropertiesPart.DataStoreItem != null &&
                    customXmlPart.CustomXmlPropertiesPart.DataStoreItem.ItemId == dataStoreItemId)
                    xmlPart = customXmlPart;
            }
            return xmlPart;
        }
    }
}