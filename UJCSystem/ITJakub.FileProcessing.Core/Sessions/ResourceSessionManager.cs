using System.Collections.Generic;
using System.IO;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceSessionManager
    {
        private readonly object m_lock = new object();

        private readonly Dictionary<string, ResourceSessionDirector> m_resourceDirectors =
            new Dictionary<string, ResourceSessionDirector>();

        private readonly ResourceProcessorManger m_resourceProcessorManager;


        public ResourceSessionManager(ResourceProcessorManger resourceProcessorManager)
        {
            m_resourceProcessorManager = resourceProcessorManager;
        }

        public void AddResource(string sessionId, string fileName, Stream dataStream)
        {
            ResourceSessionDirector director = GetDirectorBySessionId(sessionId);

            director.AddResource(fileName, dataStream);
        }

        private ResourceSessionDirector GetDirectorBySessionId(string sessionId)
        {
            ResourceSessionDirector result;

            lock (m_lock)
            {
                if (m_resourceDirectors.TryGetValue(sessionId, out result))
                    return result;

                result = new ResourceSessionDirector(sessionId);
                m_resourceDirectors.Add(sessionId, result);
            }

            return result;
        }

        public bool ProcessSession(string sessionId)
        {
            if (!m_resourceDirectors.ContainsKey(sessionId))
            {
                return false;
            }
            ResourceSessionDirector director = GetDirectorBySessionId(sessionId);

            return m_resourceProcessorManager.ProcessSessionResources(director.Resources);
        }
    }

    public class ResourceSessionDirector
    {
        public ResourceSessionDirector(string sessionId)
        {
        }

        public List<Resource> Resources { get; set; }

        public void AddResource(string fileName, Stream dataStream)
        {
        }
    }

    public class Resource
    {
    }
}