using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ITJakub.FileProcessing.DataContracts;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceSessionManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly ResourceProcessorManager m_resourceProcessorManager;
        private readonly ActiveSessionManager m_activeSessionManager;

        public ResourceSessionManager(ResourceProcessorManager resourceProcessorManager, string rootFolder, ActiveSessionManager activeSessionManager)
        {
            m_resourceProcessorManager = resourceProcessorManager;
            m_activeSessionManager = activeSessionManager;
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
        }

        public void AddResource(string sessionId, string fileName, Stream data)
        {
            ResourceSessionDirector director = m_activeSessionManager.GetDirectorBySessionId(sessionId);

            director.AddResourceAndFillResourceTypeByExtension(fileName, data);
        }
        
        public bool ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage, IList<PermissionFromAuthContract> autoImportPermissions)
        {
            if (!m_activeSessionManager.ContainsSessionId(sessionId))
            {
                return false;
            }
            
            ResourceSessionDirector director = m_activeSessionManager.GetDirectorBySessionId(sessionId);
            director.SetSessionInfoValue(SessionInfo.Message, uploadMessage);
            director.SetSessionInfoValue(SessionInfo.CreateTime, DateTime.UtcNow);
            director.SetSessionInfoValue(SessionInfo.ProjectId, projectId);
            director.SetSessionInfoValue(SessionInfo.UserId, userId);
            director.SetSessionInfoValue(SessionInfo.AutoImportPermissions, autoImportPermissions);
            bool result = m_resourceProcessorManager.ProcessSessionResources(director);
            m_activeSessionManager.FinalizeSession(sessionId);
            return result;
        }
    }
}