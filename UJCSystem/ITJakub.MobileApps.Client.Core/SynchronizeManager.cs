using System;
using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;

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

        public void SendObject(ApplicationType applicationType, string objectType, string objectValue)
        {
            m_communicationManager.SendObject(); //TODO komu, jak, atd...
        }

        public List<string> GetSynchronizedObjects(ApplicationType applicationType, DateTime from,
            string objectType = null)
        {
            return m_communicationManager.GetObjects(); //Jake, prokoho, atd...
        }
    }


    public class CommunicationManager
    {
        private ClientBase<object> m_client;

        public CommunicationManager()
        {
            //m_client = new ClientBase<object>(); //TODO swap for service reference            
        }

        public void SendObject()
        {
            throw new NotImplementedException();
        }

        public List<string> GetObjects()
        {
            throw new NotImplementedException();
        }
    }
}