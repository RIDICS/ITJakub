using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions
{
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
            m_fullPath = path;
        }

        public void AddResource(string fileName, Stream dataStream)
        {
            string fullpath = Path.Combine(m_fullPath, fileName);

            using (FileStream fs = File.Create(fullpath))
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

        public Resource GetMetaData()
        {
            throw new NotImplementedException();
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

            Directory.Delete(m_fullPath);

            m_disposed = true;
        }

        #endregion
    }
}