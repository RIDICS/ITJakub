using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceSessionDirector : IDisposable
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<SessionInfo, object> m_sessionInfos = new Dictionary<SessionInfo, object>();
        private bool m_disposed;
        private readonly ResourceTypeResolverManager m_resourceTypeResolverManager;

        public ResourceSessionDirector(string sessionId, string resourceRootFolder, ResourceTypeResolverManager resourceTypeResolverManager)
        {
            SessionId = sessionId;
            m_resourceTypeResolverManager = resourceTypeResolverManager;
            InitializeSessionFolder(resourceRootFolder);
            Resources = new List<FileResource>();
            CreateTime = DateTime.UtcNow;
        }

        public string SessionId { get; private set; }

        public string SessionPath { get; private set; }

        public DateTime CreateTime { get; private set; }

        public List<FileResource> Resources { get; set; }


        private void InitializeSessionFolder(string rootFolder)
        {
            string path = Path.Combine(rootFolder, SessionId);
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
            SessionPath = path;
        }

        public void AddResourceAndFillResourceTypeByExtension(string fileName, Stream dataStream)
        {
            string fullpath = GetFullPathForNewSessionResource(fileName);

            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
            {
                dataStream.CopyTo(fs);
            }

            var resource = new FileResource
            {
                FullPath = fullpath,
                FileName = fileName,              
            };

            AddResourceAndFillResourceTypeByExtension(resource);
        }        

        public string GetFullPathForNewSessionResource(string fileName)
        {
            return Path.Combine(SessionPath, fileName);
        }       

        public void AddResourceAndFillResourceTypeByExtension(FileResource resource)
        {
            ResourceType resourceType = m_resourceTypeResolverManager.Resolve(resource.FileName);
            resource.ResourceType = resourceType;

            Resources.Add(resource);
        }

        public FileResource GetResourceFromSession(ResourceType resourceType, string fileName)
        {
            return Resources.FirstOrDefault(x => x.ResourceType == resourceType && x.FileName == fileName);
        }

        public T GetSessionInfoValue<T>(SessionInfo sessionInfo)
        {
            return (T) m_sessionInfos[sessionInfo];
        }

        public void SetSessionInfoValue(SessionInfo sessionInfo, object value)
        {
            m_sessionInfos[sessionInfo] = value;
        }

        #region IDisposable implmentation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ResourceSessionDirector()
        {
            Dispose(false);
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

            Directory.Delete(SessionPath, true);

            m_disposed = true;
        }

        #endregion
    }

    public enum SessionInfo
    {
        CreateTime = 0,
        Message = 1,
        ProjectId = 2,
        BookXmlId = 3,
        VersionXmlId = 4,
        BookData = 5,
        UserId = 6,
        SnapshotId = 7,
        AutoImportPermissions = 8,
    }
}