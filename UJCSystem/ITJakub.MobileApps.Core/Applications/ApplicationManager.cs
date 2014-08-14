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

        public ApplicationManager(ApplicationRepository applicationRepository, UsersRepository usersRepository,
            AzureTableSynchronizedObjectDao azureTableSynchronizedObjectDao)
        {
            m_applicationRepository = applicationRepository;
            m_usersRepository = usersRepository;
            m_azureTableSynchronizedObjectDao = azureTableSynchronizedObjectDao;
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            var now = DateTime.UtcNow;

            var application = m_applicationRepository.Load<Application>(applicationId);                
            var user = m_usersRepository.Load<User>(userId);
            var group = m_usersRepository.Load<Group>(groupId);
            SynchronizedObject deSyncObject = Mapper.Map<SynchronizedObject>(synchronizedObject);
            deSyncObject.Application = application;
            deSyncObject.Author = user;
            deSyncObject.Group = group;
            deSyncObject.CreateTime = now;

            object syncObjId = m_applicationRepository.Create(deSyncObject);
            m_azureTableSynchronizedObjectDao.Create(new SynchronizedObjectEntity(syncObjId.ToString(), Convert.ToString(groupId), synchronizedObject.Data));
         
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            var syncObjs = m_applicationRepository.GetSynchronizedObjects(groupId, applicationId, objectType, since);
            
            foreach (SynchronizedObject syncObj in syncObjs) //TODO try to find some better way how to fill Data property
            {
                SynchronizedObjectEntity syncObjEntity = m_azureTableSynchronizedObjectDao.FindByRowAndPartitionKey(syncObj.Id.ToString(),
                    syncObj.Group.Id.ToString());
                if (syncObjEntity != null) 
                    syncObj.Data = syncObjEntity.Data;
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