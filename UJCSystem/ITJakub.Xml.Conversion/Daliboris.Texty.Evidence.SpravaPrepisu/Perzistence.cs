using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence
{
    public static class Perzistence
    {
        public const string csNamespace = "http://www.daliboris.cz/schemata/prepisy.xsd";
       
        public static void UlozitDoXml(Prepisy prpPrepisy, string strSoubor)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = Encoding.UTF8;
            xws.IndentChars = " ";
            xws.Indent = true;

            XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
            XmlSerializer xs = new XmlSerializer(typeof(Prepisy), csNamespace);

            if (xw != null)
            {
                xs.Serialize(xw, prpPrepisy);
                xw.Close();
            }
        }
        public static Prepisy NacistZXml(string strSoubor)
        {
            if (!File.Exists(strSoubor))
            {
                throw new FileNotFoundException("Uvedený soubor '" + strSoubor + "' neexistuje");
            }
            XmlSerializer xs = new XmlSerializer(typeof(Prepisy), csNamespace);
            FileStream fs = new FileStream(strSoubor, FileMode.Open, FileAccess.Read);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.

            // Use the Deserialize method to restore the object's state.
            Prepisy prps = (Prepisy)xs.Deserialize(reader);
            fs.Close();
            return prps;

        }

        public static void UlozitDoStreamu(Stream stmProud, Prepis prpPrepis)
        {
            XmlSerializer xs = new XmlSerializer(prpPrepis.GetType(), csNamespace);
            xs.Serialize(stmProud, prpPrepis);
        }

        /*
        public static PrepisCollection NacistZXml(string strSoubor, Type typ)
        {
            if (!File.Exists(strSoubor))
            {
                throw new FileNotFoundException("Uvedený soubor '" + strSoubor + "' neexistuje");
            }

            XmlSerializer xs = new XmlSerializer(typ, csNamespace);
            FileStream fs = new FileStream(strSoubor, FileMode.Open, FileAccess.Read);
            XmlReader reader = XmlReader.Create(fs);

            // Declare an object variable of the type to be deserialized.
            PrepisCollection prps;

            // Use the Deserialize method to restore the object's state.
            prps = (PrepisCollection)xs.Deserialize(reader);
            fs.Close();
            return prps;
        }

        public static void UlozitDoXml(PrepisCollection prpPrepisy, string strSoubor, bool blnRozlisitZdroj, Type typ)
        {


            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Encoding = System.Text.Encoding.UTF8;
            xws.IndentChars = " ";
            xws.Indent = true;

            XmlWriter xw = XmlTextWriter.Create(strSoubor, xws);
            XmlSerializer xs = new XmlSerializer(typ, csNamespace);

            xs.Serialize(xw, prpPrepisy);
            xw.Close();
            xw = null;
            xs = null;
        }
         */

    }
}
