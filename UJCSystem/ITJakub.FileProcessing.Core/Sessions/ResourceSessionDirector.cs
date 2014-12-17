using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceSessionDirector : IDisposable
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<SessionInfo, object> m_sessionInfos = new Dictionary<SessionInfo, object>();
        private bool m_disposed;

        public ResourceSessionDirector(string sessionId, string resourceRootFolder)
        {
            SessionId = sessionId;
            InitializeSessionFolder(resourceRootFolder);
            Resources = new List<Resource>();
            CreateTime = DateTime.UtcNow;
        }

        public string SessionId { get; private set; }
        public string SessionPath { get; private set; }
        public DateTime CreateTime { get; private set; }
        public List<Resource> Resources { get; set; }


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

        public void AddResource(string fileName, Stream dataStream)
        {
            string fullpath = Path.Combine(SessionPath, fileName);

            using (FileStream fs = File.Create(fullpath))
            {
                dataStream.CopyTo(fs);
            }

            ResourceTypeEnum resourceType = ResourceTypeEnum.SourceDocument; //TODO HACK make and call resourceTypeResolver by file extension

            var resource = new Resource
            {
                FullPath = fullpath,
                FileName = fileName,
                ResourceType = resourceType
            };

            Resources.Add(resource);
        }

        public T GetSessionInfoValue<T>(SessionInfo sessionInfo)
        {
            return (T) m_sessionInfos[sessionInfo];
        }

        public void SetSessionInfoValue(SessionInfo sessionInfo, object value)
        {
            m_sessionInfos.Add(sessionInfo, value);
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

            Directory.Delete(SessionPath);

            m_disposed = true;
        }

        #endregion
    }

    public enum SessionInfo
    {
        CreateTime = 0,
        Message = 1,
        VersionId = 2,
        BookId = 3,
        BookVersionEntity = 4

    }
}