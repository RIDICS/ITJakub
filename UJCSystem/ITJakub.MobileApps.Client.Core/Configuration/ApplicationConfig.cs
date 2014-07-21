using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ITJakub.MobileApps.Client.Core.Configuration
{
    public class ApplicationConfig
    {
        public List<string> ApplicationAssemblies { get; set; }


        public virtual string ToXml()
        {
            using (Stream stream = new MemoryStream())
            {
                new XmlSerializer(GetType()).Serialize(stream, this);

                stream.Position = 0;
                string result = new StreamReader(stream).ReadToEnd();

                return result;
            }
        }

        /// <summary> 
        /// Deserialize config from string 
        /// </summary> 
        /// <param name="xml"></param> 
        /// <param name="messageType"></param> 
        /// <returns></returns> 
        public static ApplicationConfig FromXml(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(xml);
                writer.Flush();

                stream.Position = 0;
                object result = new XmlSerializer(typeof(ApplicationConfig)).Deserialize(stream);

                return (ApplicationConfig)result;
            }
        }
    }
}
