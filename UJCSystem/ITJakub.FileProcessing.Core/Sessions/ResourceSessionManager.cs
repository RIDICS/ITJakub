using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceSessionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_rootFolderPath;
        private readonly object m_lock = new object();

        private readonly Dictionary<string, ResourceSessionDirector> m_resourceDirectors =
            new Dictionary<string, ResourceSessionDirector>();

        private readonly ResourceProcessorManager m_resourceProcessorManager;

        public ResourceSessionManager(ResourceProcessorManager resourceProcessorManager, string rootFolder)
        {
            m_resourceProcessorManager = resourceProcessorManager;
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
            var director = GetDirectorBySessionId(resourceInfoSkeleton.SessionId);

            director.AddResource(resourceInfoSkeleton.FileName, resourceInfoSkeleton.Data);
        }

        private ResourceSessionDirector GetDirectorBySessionId(string sessionId)
        {
            ResourceSessionDirector result;

            lock (m_lock)
            {
                if (m_resourceDirectors.TryGetValue(sessionId, out result))
                    return result;

                result = new ResourceSessionDirector(sessionId, m_rootFolderPath);
                m_resourceDirectors.Add(sessionId, result);
            }

            return result;
        }

        public bool ProcessSession(string sessionId)
        {
            lock (m_lock)
            {
                if (!m_resourceDirectors.ContainsKey(sessionId))
                {
                    return false;
                }
            }
            var director = GetDirectorBySessionId(sessionId);

            return m_resourceProcessorManager.ProcessSessionResources(director);
        }
    }

    public class ResourceSessionDirector : IDisposable
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool m_disposed;
        private string m_fullPath;

        public ResourceSessionDirector(string sessionId, string resourceRootFolder)
        {
            SessionId = sessionId;
            InitializeSessionFolder(resourceRootFolder);
            Resources = new List<Resource>();
            CreateTime = DateTime.UtcNow;
        }

        public string SessionId { get; private set; }
        public DateTime CreateTime { get; private set; }
        public List<Resource> Resources { get; set; }

       

        private void InitializeSessionFolder(string rootFolder)
        {
            var path = Path.Combine(rootFolder, SessionId);
            if (!Directory.Exists(path))
            {
                if (m_log.IsInfoEnabled)
                    m_log.InfoFormat("Creating directory for resources in: '{0}'", path);

                try
                {
                    Directory.CreateDirectory(path);
                   
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
            m_fullPath = path;
        }

        public void AddResource(string fileName, Stream dataStream)
        {
            var fullpath = Path.Combine(m_fullPath, fileName);

            using (var fs = File.Create(fullpath))
            {
                dataStream.CopyTo(fs);
            }

            var resource = new Resource
            {
                FullPath = fullpath,
                FileName = fileName
            };

            Resources.Add(resource);
        }

        #region IDisposable implmentation
        ~ResourceSessionDirector()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
            }

            Directory.Delete(m_fullPath);

            m_disposed = true;
        }
        #endregion

        public Resource GetMetaData()
        {
            throw new NotImplementedException();
        }
    }

    public class Resource
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }
}