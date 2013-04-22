using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Diagnostics;
using Ujc.Naki.DataLayer;
using AdvLib;

namespace ServicesLayer
{
    [ServiceContract]
    public class ValidationService
    {

        // TODO DI?
        private IXmlDbDao dao = new ExistDao();
        private IXmlFormatDescriptor formatDesc = new TeiP5Descriptor();

        /// <summary>
        /// Validates specified ADV file.
        /// </summary>
        /// <param name="advFile">ADV file to be validated</param>
        /// <returns>true if validation was successful, otherwise false</returns>
        [OperationContract]
        public bool ValidateAdvFile(AdvFile advFile)
        {
            return advFile.Validate();
        }

        /// <summary>
        /// Validates specified XML.
        /// </summary>
        /// <param name="xml">XML to be validated</param>
        /// <returns>true if validation was successful, otherwise false</returns>
        [OperationContract]
        public bool ValidateXml(string xml)
        {
            // add some cache for validators?

            DocumentKind kind = DocumentKind.Prose; 
            
            // TODO get from XML somehow?
            // Waiting for Boris Lehecka opinion about TEI P5 standard.
            // The information of document kind (edition, vocabulary etc...) should be present somewhere in the TEI P5 XML.

            IEnumerable<string> xsds = dao.ReadXsds(kind);
            try
            {
                XmlReaderSettings xmlSettings = new XmlReaderSettings();
                xmlSettings.Schemas = new System.Xml.Schema.XmlSchemaSet();
                foreach (string xsd in xsds)
                {
                    XmlSchema schema = XmlSchema.Read(new StringReader(xsd), null);
                    xmlSettings.Schemas.Add(schema);
                }
                xmlSettings.ValidationType = ValidationType.Schema;
                xmlSettings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(new StringReader(xml), xmlSettings);

                while (reader.Read()) ;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
