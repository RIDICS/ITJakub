using System.Collections.Generic;
using AutoMapper;
using ITJakub.CardFile.Core;
using ITJakub.CardFile.Core.DataContractEntities;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class CardFileManager
    {
        private readonly CardFilesCommunicationManager m_cardFileClient;

        public CardFileManager()
        {
            m_cardFileClient = new CardFilesCommunicationManager(); //TODO load from container
        }

        public IList<CardFileContract> GetCardFiles()
        {
            var cardFiles = m_cardFileClient.GetFiles();
            return Mapper.Map<file[], IList<CardFileContract>>(cardFiles.file);
        }
    }
}