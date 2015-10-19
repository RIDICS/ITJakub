using System;
using System.Collections.Generic;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.News;

namespace ITJakub.ITJakubService.Core
{
    public class NewsManager
    {
        private readonly NewsRepository m_repository;
        private readonly UserRepository m_userRepository;
        private readonly AuthorizationManager m_authorizationManager;

        public NewsManager(NewsRepository repository, UserRepository userRepository, AuthorizationManager authorizationManager)
        {
            m_repository = repository;
            m_userRepository = userRepository;
            m_authorizationManager = authorizationManager;
        }

        public List<NewsSyndicationItemContract> GetWebNewsSyndicationItems(int start, int count)
        {            
            var syndicationItems = m_repository.GetWebNews(start, count);
            return Mapper.Map<List<NewsSyndicationItemContract>>(syndicationItems);
        }

        public int GetWebNewsSyndicationItemCount()
        {
            var syndicationItemCount = m_repository.GetWebNewsSyndicationItemCount();
            return syndicationItemCount;
        }

        public void CreateNewSyndicationItem(string title, string content, string url, NewsTypeContract itemType, string username)
        {
            m_authorizationManager.CheckUserCanAddNews();
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is empty, cannot add bookmark");

            User user = m_userRepository.FindByUserName(username);
            if (user == null)
                throw new ArgumentException(string.Format("Cannot locate user by username: '{0}'", username));

            NewsSyndicationItem syndicationItem = new NewsSyndicationItem
            {
                CreateDate = DateTime.UtcNow,
                Title = title,
                Url = url,
                Text = content,
                ItemType = Mapper.Map<SyndicationItemType>(itemType),
                User = user,
            };

            m_repository.Save(syndicationItem);
        }


        public IList<MobileApps.MobileContracts.News.NewsSyndicationItemContract> GetNewsForMobileApps(int start, int count)
        {
            var syndicationItems = m_repository.GetMobileAppsNews(start, count);
            return Mapper.Map<List<MobileApps.MobileContracts.News.NewsSyndicationItemContract>>(syndicationItems);
        }
    }
}