﻿using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class ResourceManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly UserDetailManager m_userDetailManager;

        public ResourceManager(ResourceRepository resourceRepository, UserDetailManager userDetailManager)
        {
            m_resourceRepository = resourceRepository;
            m_userDetailManager = userDetailManager;
        }

        public IList<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetResourceVersionHistory(resourceId));

            var resultList = new List<ResourceVersionContract>();
            var userCache = new Dictionary<int, string>();
            foreach (var resource in dbResult)
            {
                var resourceContract = Mapper.Map<ResourceVersionContract>(resource);
                var userId = resource.CreatedByUser.Id;
                if (!userCache.TryGetValue(userId, out var userName))
                {
                    userCache.Add(userId, m_userDetailManager.GetUserName(resource.CreatedByUser));
                    userCache.TryGetValue(userId, out userName);
                }

                resourceContract.Author = userName;
                resultList.Add(resourceContract);
            }

            return resultList;
        }
    }
}
