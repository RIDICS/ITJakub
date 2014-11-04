using System;
using System.IO;

namespace ITJakub.XmlProcessingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream xmlFileStream = File.Open("D:\\ITJakubTestXml\\Albrecht.xml", FileMode.Open);
            var xmlProcessingManager = new XmlProcessingManager();
            var bookVerionn = xmlProcessingManager.GetMetadataFromXml(xmlFileStream);
            Console.WriteLine("XML sucessfully parsed!");
            Console.ReadKey();
        }
    }
}
