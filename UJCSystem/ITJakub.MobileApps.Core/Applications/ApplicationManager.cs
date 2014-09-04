using System;
using System.Collections.Generic;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataEntities.AzureTables.Daos;
using ITJakub.MobileApps.DataEntities.AzureTables.Entities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Applications
{
    public class ApplicationManager
    {
        private readonly ApplicationRepository m_applicationRepository;
        private readonly AzureTableSynchronizedObjectDao m_azureTableSynchronizedObjectDao;
        private readonly UsersRepository m_usersRepository;
        private readonly AzureTableIdGenerator m_idGenerator;

        public ApplicationManager(ApplicationRepository applicationRepository, UsersRepository usersRepository, AzureTableSynchronizedObjectDao azureTableSynchronizedObjectDao, AzureTableIdGenerator idGenerator)
        {
            m_applicationRepository = applicationRepository;
            m_usersRepository = usersRepository;
            m_azureTableSynchronizedObjectDao = azureTableSynchronizedObjectDao;
            m_idGenerator = idGenerator;
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            var syncObjectEntity = new SynchronizedObjectEntity(m_idGenerator.GetNewId(), Convert.ToString(groupId), synchronizedObject.Data);
            m_azureTableSynchronizedObjectDao.Create(syncObjectEntity);

            var now = DateTime.UtcNow;

            var application = m_applicationRepository.Load<Application>(applicationId);                
            var user = m_usersRepository.Load<User>(userId);
            var group = m_usersRepository.Load<Group>(groupId);
            SynchronizedObject deSyncObject = Mapper.Map<SynchronizedObject>(synchronizedObject);
            deSyncObject.Application = application;
            deSyncObject.Author = user;
            deSyncObject.Group = group;
            deSyncObject.CreateTime = now;
            deSyncObject.RowKey = syncObjectEntity.RowKey;

            m_applicationRepository.Create(deSyncObject);
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            var syncObjs = m_applicationRepository.GetSynchronizedObjects(groupId, applicationId, objectType, since);
            
            foreach (SynchronizedObject syncObj in syncObjs) //TODO try to find some better way how to fill Data property
            {
                SynchronizedObjectEntity syncObjEntity = m_azureTableSynchronizedObjectDao.FindByRowAndPartitionKey(syncObj.RowKey,
                    Convert.ToString(syncObj.Group.Id));
                if (syncObjEntity != null && syncObjEntity.Data != null)
                    syncObj.Data = syncObjEntity.Data;
                else
                    throw new ArgumentException("TEST");

            }
            return Mapper.Map<IList<SynchronizedObjectResponseContract>>(syncObjs);
        }

        public IList<ApplicationContract> GetAllApplication()
        {
            var apps = m_applicationRepository.GetAllApplication();
            return Mapper.Map<IList<ApplicationContract>>(apps);
        }
    }
}