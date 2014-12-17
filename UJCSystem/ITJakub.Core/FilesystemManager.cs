using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Castle.MicroKernel;
using ITJakub.Core.PathResolvers;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.Core
{
    public class FileSystemManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<ResourceTypeEnum, IResourceTypePathResolver> m_resourceTypePathResolvers;
        private readonly string m_rootFolderPath;

        public FileSystemManager(IKernel container, string rootFolder)
        {
            m_rootFolderPath = rootFolder;
            m_resourceTypePathResolvers = new Dictionary<ResourceTypeEnum, IResourceTypePathResolver>();
            foreach (var pathResolver in container.ResolveAll<IResourceTypePathResolver>())
            {
                m_resourceTypePathResolvers.Add(pathResolver.ResolvingResourceType(), pathResolver);
            }
        }

        public Resource GetResource(string bookId, string bookVersionId, string fileName, ResourceTypeEnum resourceType)
        {
            var pathResolver = GetPathResolver(resourceType);
            var relativePath = pathResolver.ResolvePath(bookId, bookVersionId, fileName);
            return new Resource
            {
                FileName = fileName,
                FullPath = GetFullPath(relativePath),
                ResourceType = resourceType
            };
        }

        public void SaveResource(string bookId, string bookVersionId, Resource resource)
        {
            var pathResolver = GetPathResolver(resource.ResourceType);
            var relativePath = pathResolver.ResolvePath(bookId, bookVersionId, resource.FileName);
            var fullPath = GetFullPath(relativePath);
            CreateDirsIfNotExist(fullPath);
            using (var sourceStream = File.Open(resource.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var writeStream = File.Create(fullPath))
                {
                    sourceStream.CopyTo(writeStream);
                }
            }
        }

        private string GetFullPath(string relativePath)
        {
            return Path.Combine(m_rootFolderPath, relativePath);
        }

        private void CreateDirsIfNotExist(string path)
        {
            var dirPath = Path.GetDirectoryName(path);
            if (dirPath == null) return;
            if (!Directory.Exists(path))
            {
                if (m_log.IsInfoEnabled)
                    m_log.InfoFormat("Creating directory for resources in: '{0}'", dirPath);

                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (IOException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", dirPath,
                            ex);
                    throw;
                }
                catch (UnauthorizedAccessException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", dirPath,
                            ex);
                    throw;
                }
                catch (NotSupportedException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", dirPath,
                            ex);
                    throw;
                }
                catch (ArgumentException ex)
                {
                    if (m_log.IsFatalEnabled)
                        m_log.FatalFormat("Cannot create directory on path: '{0}'. Exception: '{1}'", dirPath,
                            ex);
                    throw;
                }
            }
        }

        private IResourceTypePathResolver GetPathResolver(ResourceTypeEnum resourceType)
        {
            IResourceTypePathResolver pathResolver;
            m_resourceTypePathResolvers.TryGetValue(resourceType, out pathResolver);
            if (pathResolver == null)
            {
                var message = string.Format("Resource with type '{0}' does not have rule for resolving path",
                    resourceType);
                if (m_log.IsFatalEnabled)
                    m_log.FatalFormat(message);
                throw new InvalidEnumArgumentException(message);
            }
            return pathResolver;
        }
    }
}