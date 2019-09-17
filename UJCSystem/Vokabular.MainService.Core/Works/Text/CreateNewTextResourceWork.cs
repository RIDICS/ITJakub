using System;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Text
{
    public class CreateNewTextResourceWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly CreateTextRequestContract m_newTextContract;
        private readonly int m_userId;
        private readonly IFulltextStorage m_fulltextStorage;

        public CreateNewTextResourceWork(ResourceRepository resourceRepository, CreateTextRequestContract newTextContract, int userId, IFulltextStorage fulltextStorage) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_newTextContract = newTextContract;
            m_userId = userId;
            m_fulltextStorage = fulltextStorage;
        }

        protected override long ExecuteWorkImplementation()
        {
            var timeNow = DateTime.UtcNow;
            var latestVersion = m_resourceRepository.GetTextResource(m_newTextContract.Id);

            if (latestVersion == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, $"TextResource with ResourceId={m_newTextContract.Id} was not found");
            }

            if (latestVersion.VersionNumber != m_newTextContract.VersionNumber)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.ChangeInConflict,
                    $"Conflict. Current latest versionNumber is {latestVersion.Id}, but originalVersionNumber was specified {m_newTextContract.VersionNumber}",
                    HttpStatusCode.Conflict
                );
            }

            var newVersion = new TextResource
            {
                CreatedByUser = m_resourceRepository.Load<User>(m_userId),
                CreateTime = timeNow,
                ExternalId = null,
                ResourcePage = latestVersion.ResourcePage,
                Resource = latestVersion.Resource,
                VersionNumber = latestVersion.VersionNumber + 1,
            };
            newVersion.Resource.LatestVersion = newVersion;

            var result = (long) m_resourceRepository.Create(newVersion);

            var externalTextId = m_fulltextStorage.CreateNewTextVersion(newVersion, m_newTextContract.Text);

            newVersion.ExternalId = externalTextId;
            m_resourceRepository.Update(newVersion);

            return result;
        }
    }
}