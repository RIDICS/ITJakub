using System;
using NLipsum.Core;
using Vokabular.MainService.DataContracts.Clients;

namespace Vokabular.ImportTestData.App
{
    public class ImportTestProjectManager
    {
        private readonly MainServiceProjectClient m_projectClient;

        private readonly string[] m_dataSets = {
            Lipsums.LoremIpsum,
            Lipsums.ChildHarold,
            Lipsums.Decameron,
            Lipsums.Faust,
            Lipsums.InDerFremde,
            Lipsums.LeBateauIvre,
            Lipsums.LeMasque,
            Lipsums.NagyonFaj,
            Lipsums.Omagyar,
            Lipsums.RobinsonoKruso,
            Lipsums.TheRaven,
            Lipsums.TierrayLuna,
        };
        
        public ImportTestProjectManager(MainServiceProjectClient projectClient)
        {
            m_projectClient = projectClient;
        }

        public void Import(int index)
        {
            var rawDataSet = m_dataSets[index % m_dataSets.Length];
            var text = LipsumGenerator.Generate(2, rawDataSet);

            Console.WriteLine(text);
            Console.WriteLine("#########################");
            
            // TODO create project, metadata, pages, texts
        }
    }
}