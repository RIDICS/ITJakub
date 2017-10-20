using System;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Text
{
    public class CreateNewTextResourceWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly ShortTextContract m_newTextContract;
        private readonly int m_userId;
        private readonly CommunicationProvider m_communicationProvider;

        public CreateNewTextResourceWork(ResourceRepository resourceRepository, ShortTextContract newTextContract, int userId, CommunicationProvider communicationProvider) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_newTextContract = newTextContract;
            m_userId = userId;
            m_communicationProvider = communicationProvider;
        }

        protected override long ExecuteWorkImplementation()
        {
            var timeNow = DateTime.UtcNow;
            var latestVersion = m_resourceRepository.GetTextResource(m_newTextContract.Id);
            //TODO check version conflict

            var client = m_communicationProvider.GetFulltextServiceClient();
            var textId = client.CreateTextResource(m_newTextContract.Text);
            //TODO check succes 
            var newVersion = new TextResource
            {
                CreatedByUser = m_resourceRepository.Load<User>(m_userId),
                CreateTime = timeNow,
                ExternalId = textId,//TODO fill from elastic
                ParentResource = latestVersion.ParentResource,
                Resource = latestVersion.Resource,
                VersionNumber = latestVersion.VersionNumber + 1,
            };
            newVersion.Resource.LatestVersion = newVersion;
            var result = (long)m_resourceRepository.Create(newVersion);
            return result;
        }
    }
}