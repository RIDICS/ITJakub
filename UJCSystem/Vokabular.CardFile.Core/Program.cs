using System;
using Vokabular.Shared.DataContracts.Attribute;

namespace Vokabular.CardFile.Core
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var service = new CardFilesCommunicationManager(null, null); //TODO probably remove this class

            var files = service.GetFiles();
            //var buckets = service.GetBuckets(2.ToString(), null);
            ////var bucket = service.GetCardsFromBucket(1.ToString(), 2080.ToString());
            //var card = service.GetCardFromBucket(1.ToString(), 2080.ToString(), 7968548.ToString());

            var image = service.GetImageForCard("2", "71","112740", "0_768432_44783_667", CardServiceImageTypes.Full.GetStringValue());
            Console.ReadKey();
        }
    }
}