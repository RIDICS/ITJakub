using System;

namespace ITJakub.XmlProcessingConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new ItJakubServiceClient();
            client.TestXml();
            Console.WriteLine("XML sucessfully parsed!");
            Console.ReadKey();
        }
    }
}
