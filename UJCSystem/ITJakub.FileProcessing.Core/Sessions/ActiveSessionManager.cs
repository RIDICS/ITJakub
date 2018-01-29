using System;
using System.Collections.Generic;
using System.IO;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ActiveSessionManager : IDisposable
    {
        private readonly object m_lock = new object();
        private readonly Dictionary<string, ResourceSessionDirector> m_resourceDirectors =
            new Dictionary<string, ResourceSessionDirector>();
        private readonly string m_rootFolderPath;
        private readonly ResourceTypeResolverManager m_resourceTypeResolverManager;
        private bool m_disposed;

        public ActiveSessionManager(string rootFolder, ResourceTypeResolverManager resourceTypeResolverManager)
        {
            m_rootFolderPath = rootFolder;
            m_resourceTypeResolverManager = resourceTypeResolverManager;
        }

        public bool ContainsSessionId(string sessionId)
        {
            lock (m_lock)
            {
                return m_resourceDirectors.ContainsKey(sessionId);
            }
        }

        public ResourceSessionDirector GetDirectorBySessionId(string sessionId)
        {
            ResourceSessionDirector result;

            lock (m_lock)
            {
                if (m_resourceDirectors.TryGetValue(sessionId, out result))
                    return result;

                result = new ResourceSessionDirector(sessionId, m_rootFolderPath, m_resourceTypeResolverManager);
                m_resourceDirectors.Add(sessionId, result);
            }

            return result;
        }

        public void FinalizeSession(string sessionId)
        {
            lock (m_lock)
            {
                m_resourceDirectors[sessionId].Dispose();
                m_resourceDirectors.Remove(sessionId);
            }
        }

        #region IDisposable implmentation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ActiveSessionManager()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                lock (m_lock)
                {
                    foreach (ResourceSessionDirector resourceDirector in m_resourceDirectors.Values)
                    {
                        resourceDirector.Dispose();
                    }
                }
            }

            if (Directory.Exists(m_rootFolderPath))
            {
                Directory.Delete(m_rootFolderPath, true);
            }

            m_disposed = true;
        }

        #endregion
    }
}