using System;
using System.Collections.Generic;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Applications;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using ITJakub.MobileApps.DataEntities.ExternalEntities;
using ITJakub.MobileApps.DataEntities.ExternalEntities.AzureTables;

namespace ITJakub.MobileApps.Core.Applications
{
    public class ApplicationManager
    {
        private readonly ApplicationRepository m_applicationRepository;
        private readonly ISynchronizedObjectDao m_synchronizedObjectDataProvider;
        private readonly UsersRepository m_usersRepository;
        private readonly AzureTableIdGenerator m_idGenerator;

        public ApplicationManager(ApplicationRepository applicationRepository, UsersRepository usersRepository, ISynchronizedObjectDao synchronizedObjectDataProvider, AzureTableIdGenerator idGenerator)
        {
            m_applicationRepository = applicationRepository;
            m_usersRepository = usersRepository;
            m_synchronizedObjectDataProvider = synchronizedObjectDataProvider;
            m_idGenerator = idGenerator;
        }

        public void CreateSynchronizedObject(int applicationId, long groupId, long userId,
            SynchronizedObjectContract synchronizedObject)
        {
            switch (synchronizedObject.SynchronizationType)
            {
                case SynchronizationTypeContract.HistoryTrackingObject:
                    CreateHistoryTrackingObject(applicationId, groupId, userId, synchronizedObject);
                    break;
                case SynchronizationTypeContract.SingleObject:
                    CreateSingleSynchronizeObject(applicationId, groupId, userId, synchronizedObject);
                    break;
            }
        }

        private void CreateSingleSynchronizeObject(int applicationId, long groupId, long userId, SynchronizedObjectContract synchronizedObject)
        {            
            var syncObject = m_applicationRepository.GetLatestSynchronizedObject(groupId, applicationId, synchronizedObject.ObjectType, new DateTime(1975,1,1));

            var now = DateTime.UtcNow;
            var user = m_usersRepository.Load<User>(userId);

            if (syncObject != null)
            {
                syncObject.Author = user;
                syncObject.CreateTime = now;
                syncObject.ObjectValue = synchronizedObject.Data;
                m_applicationRepository.Save(syncObject);
                return;
            }
            
            var group = m_usersRepository.Load<Group>(groupId);
            var application = m_applicationRepository.Load<Application>(applicationId);
            
            var deSyncObject = new SingleSynchronizedObject
            {
                Application = application,
                Author = user,
                Group = group,
                CreateTime = now,
                ObjectType = synchronizedObject.ObjectType,
                ObjectValue = synchronizedObject.Data
            };

            m_applicationRepository.Create(deSyncObject);
        }

        private void CreateHistoryTrackingObject(int applicationId, long groupId, long userId, SynchronizedObjectContract synchronizedObject)
        {
            var group = m_usersRepository.Load<Group>(groupId);
            var syncObjectEntity = m_synchronizedObjectDataProvider.GetNewEntity(groupId, synchronizedObject.Data);

            m_synchronizedObjectDataProvider.Save(syncObjectEntity);

            var now = DateTime.UtcNow;

            var application = m_applicationRepository.Load<Application>(applicationId);
            var user = m_usersRepository.Load<User>(userId);

            var deSyncObject = new SynchronizedObject
            {
                Application = application,
                Author = user,
                Group = @group,
                CreateTime = now,
                ObjectExternalId = syncObjectEntity.ExternalId,
                ObjectType = synchronizedObject.ObjectType
            };

            m_applicationRepository.Create(deSyncObject);
        }

        public IList<SynchronizedObjectResponseContract> GetSynchronizedObjects(long groupId, int applicationId, string objectType, DateTime since)
        {
            var syncObjs = m_applicationRepository.GetSynchronizedObjects(groupId, applicationId, objectType, since);

            foreach (SynchronizedObject syncObj in syncObjs) //TODO try to find some better way how to fill Data property
            {
                ISynchronizedObjectEntity syncObjEntity = m_synchronizedObjectDataProvider.FindByObjectExternalIdAndGroup(syncObj.ObjectExternalId,
                    syncObj.Group.Id);

                syncObj.Data = syncObjEntity.Data;
            }

            return Mapper.Map<IList<SynchronizedObjectResponseContract>>(syncObjs);
        }

        public IList<ApplicationContract> GetAllApplication()
        {
            var apps = m_applicationRepository.GetAllApplication();
            return Mapper.Map<IList<ApplicationContract>>(apps);
        }

        public void DeleteSynchronizedObjects(long groupId, IEnumerable<string> externalIds)
        {
            m_synchronizedObjectDataProvider.DeleteSynchronizedObjects(groupId, externalIds);       
        }

        public SynchronizedObjectResponseContract GetLatestSynchronizedObject(long groupId, int applicationId, string objectType, DateTime since)
        {
            var syncObj = m_applicationRepository.GetLatestSynchronizedObject(groupId, applicationId, objectType, since);
            if (syncObj == null)
                return null;

            return Mapper.Map<SynchronizedObjectResponseContract>(syncObj);
        }
    }
}