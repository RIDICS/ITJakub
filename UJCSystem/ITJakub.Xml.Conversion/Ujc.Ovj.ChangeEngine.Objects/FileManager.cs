using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    public class FileManager
    {

        public ChangedTokens LoadChangedTokens(string filePath)
        {
            return DeserializeObjectFromXml<ChangedTokens>(filePath) as ChangedTokens;
        }
        public void SaveChangedTokens(ChangedTokens changedTokens, string filePath)
        {
            SerializeObjectToXml(changedTokens, filePath);
        }

        public Changes LoadChanges(string filePath)
        {
            return DeserializeObjectFromXml<Changes>(filePath) as Changes;
        }

        public void SaveChanges(Changes changes, string filePath)
        {
            SaveChanges(changes, filePath, false);
        }

        public void SaveChanges(Changes changes, string filePath, bool indent)
        {
            SerializeObjectToXml(changes, filePath, indent);
        }


        public ChangeRuleSet LoadChangeRuleSet(string filePath)
        {
            return DeserializeObjectFromXml<ChangeRuleSet>(filePath) as ChangeRuleSet;
        }

        public ChangeRuleSet LoadChangeRuleSet(Stream fileStream)
        {
            return DeserializeObjectFromXml<ChangeRuleSet>(fileStream) as ChangeRuleSet;
        }

        public void SaveChangeRuleSet(ChangeRuleSet ruleset, string filePath)
        {
            SaveChangeRuleSet(ruleset, filePath, false);
        }

        public void SaveChangeRuleSet(ChangeRuleSet ruleset, string filePath, bool indent)
        {
            SerializeObjectToXml(ruleset, filePath, indent);
        }

        

        public static object DeserializeObjectFromXml<T>(string filePath)
        {
            return DeserializeObjectFromXml<T>(new StreamReader(filePath));
        }

        public static object DeserializeObjectFromXml<T>(Stream fileStream)
        {
            return DeserializeObjectFromXml<T>(new StreamReader(fileStream));
        }

        public static object DeserializeObjectFromXml<T>(StreamReader sr)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            
            return (T)xs.Deserialize(sr);
        }

        public static void SerializeObjectToXml<T>(T item, string filePath)
        {
            SerializeObjectToXml(item, filePath, false);
        }

        public static void SerializeObjectToXml<T>(T item, string filePath, bool indent)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.CloseOutput = true;
            xws.Indent = indent;

            XmlWriter xw = XmlWriter.Create(filePath, xws);
            xs.Serialize(xw, item);
            xw.Close();
        }
    }
}
