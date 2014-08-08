using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ITJakub.MobileApps.Client.Core.Service;
using ITJakub.MobileApps.Client.DataContracts;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Data;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core
{
    public class SynchronizeManager : ISynchronizeCommunication
    {
        private static readonly SynchronizeManager m_instance = new SynchronizeManager();
        private readonly CommunicationManager m_communicationManager = new CommunicationManager();

        private SynchronizeManager()
        {
        }

        public static SynchronizeManager Instance
        {
            get { return m_instance; }
        }

        public string UserId { get; set; }
        public string GroupId { get; set; }

        public void SendObject(ApplicationType applicationType, string objectType, string objectValue)
        {
            m_communicationManager.SendObject(applicationType, GroupId, UserId, objectType, objectValue);
        }

        public ObservableCollection<ObjectDetails> GetSynchronizedObjects(ApplicationType applicationType, DateTime since,
            string objectType = null)
        {
            var dateTimeString = since.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            var synchronizedObjects = m_communicationManager.GetObjects(applicationType, GroupId, objectType, dateTimeString);
            var list = new ObservableCollection<ObjectDetails>();
            foreach (var item in synchronizedObjects)
            {
                list.Add(new ObjectDetails
                {
                    Author = new AuthorInfo
                    {
                        Email = item.Author.User.Email,
                        FirstName = item.Author.User.FirstName,
                        LastName = item.Author.User.LastName
                    },
                    CreateTime = item.CreateTime,
                    Data = item.SynchronizedObject.Data,
                    Id = item.Id
                });
            }
            return list;
        }
    }


    public class CommunicationManager
    {
        private readonly MobileAppsServiceClient m_client;

        public CommunicationManager()
        {
            m_client = new MobileAppsServiceClient();
        }

        public async void SendObject(ApplicationType applicationType, string groupId, string userId, string objectType, string objectValue)
        {
            var applicationId = ((int) applicationType).ToString();
            var synchronizedObject = new SynchronizedObject
            {
                ObjectType = objectType,
                Data = objectValue
            };
            await m_client.CreateSynchronizedObjectAsync(groupId, applicationId, userId, synchronizedObject);
        }

        public IEnumerable<SynchronizedObjectDetails> GetObjects(ApplicationType applicationType, string groupId, string objectType, string since)
        {
            var applicationId = ((int) applicationType).ToString();
            return m_client.GetSynchronizedObjectsAsync(groupId, applicationId, objectType, since).Result;
        }
    }
}