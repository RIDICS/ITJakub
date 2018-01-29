using System;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Portal
{
    public class CreateNewsWork : UnitOfWorkBase<long>
    {
        private readonly PortalRepository m_portalRepository;
        private readonly CreateNewsSyndicationItemContract m_data;
        private readonly int m_userId;

        public CreateNewsWork(PortalRepository portalRepository, CreateNewsSyndicationItemContract data, int userId) : base(portalRepository)
        {
            m_portalRepository = portalRepository;
            m_data = data;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            // TODO add authorization
            //m_authorizationManager.CheckUserCanAddNews();
            //if (string.IsNullOrWhiteSpace(username))
            //    throw new ArgumentException("Username is empty, cannot add bookmark");

            var now = DateTime.UtcNow;
            var user = m_portalRepository.Load<User>(m_userId);
            var itemType = Mapper.Map<SyndicationItemType>(m_data.ItemType);

            var syndicationItem = new NewsSyndicationItem
            {
                CreateTime = now,
                Title = m_data.Title,
                Url = m_data.Url,
                Text = m_data.Text,
                ItemType = itemType,
                User = user,
            };

            var resultId = (long) m_portalRepository.Create(syndicationItem);
            return resultId;
        }
    }
}

