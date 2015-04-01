using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceSessionManager : IDisposable
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly object m_lock = new object();

        private readonly Dictionary<string, ResourceSessionDirector> m_resourceDirectors =
            new Dictionary<string, ResourceSessionDirector>();

        private readonly ResourceProcessorManager m_resourceProcessorManager;
        private bool m_disposed;
        private string m_rootFolderPath;
        private readonly ResourceTypeResolverManager m_resourceTypeResolverManager;

        public ResourceSessionManager(ResourceProcessorManager resourceProcessorManager, string rootFolder, ResourceTypeResolverManager resourceTypeResolverManager)
        {
            m_resourceProcessorManager = resourceProcessorManager;
            m_resourceTypeResolverManager = resourceTypeResolverManager;
            InitializeRootFolder(rootFolder);
        }

        private void InitializeRootFolder(string rootFolder)
        {
            if (!Directory.Exists(rootFolder))
            {
                if (m_log.IsInfoEnabled)
                    m_log.InfoFormat("Creating directory for resources in: '{0}'", rootFolder);

                try
                {
                    Directory.CreateDirectory(rootFolder);
                }
                catch (IOException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", rootFolder, ex);
                    throw;
                }
                catch (UnauthorizedAccessException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", rootFolder, ex);
                    throw;
                }
                catch (NotSupportedException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", rootFolder, ex);
                    throw;
                }
                catch (ArgumentException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", rootFolder, ex);
                    throw;
                }
            }
            m_rootFolderPath = rootFolder;
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            ResourceSessionDirector director = GetDirectorBySessionId(resourceInfoSkeleton.SessionId);

            director.AddResource(resourceInfoSkeleton.FileName, resourceInfoSkeleton.Data);
        }

        private ResourceSessionDirector GetDirectorBySessionId(string sessionId)
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

        public bool ProcessSession(string sessionId, string uploadMessage)
        {
            lock (m_lock)
            {
                if (!m_resourceDirectors.ContainsKey(sessionId))
                {
                    return false;
                }
            }
            ResourceSessionDirector director = GetDirectorBySessionId(sessionId);
            director.SetSessionInfoValue(SessionInfo.Message,  uploadMessage);
            director.SetSessionInfoValue(SessionInfo.CreateTime,  DateTime.UtcNow);
            bool result = m_resourceProcessorManager.ProcessSessionResources(director);
            FinalizeSession(sessionId);
            return result;
        }

        private void FinalizeSession(string sessionId)
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

        ~ResourceSessionManager()
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