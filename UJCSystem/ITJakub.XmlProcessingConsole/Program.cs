using System;
using ITJakub.Core;

namespace ITJakub.XmlProcessingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var searchService = new SearchServiceClient();
            var result = searchService.GetBookPageByPosition("{125A0032-03B5-40EC-B68D-80473CC5653A}", 1);
            var result2 = searchService.GetBookPagesByName("{125A0032-03B5-40EC-B68D-80473CC5653A}", "2r", "3v");
            var result3 = searchService.GetBookPageByName("{125A0032-03B5-40EC-B68D-80473CC5653A}", "2r");
            //var client = new ItJakubServiceClient();
            //client.TestXml();
            //Console.WriteLine("XML sucessfully parsed!");
            //Console.ReadKey();
        }
    }
}
