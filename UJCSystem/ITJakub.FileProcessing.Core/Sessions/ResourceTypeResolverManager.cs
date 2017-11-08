using System;
using System.Collections.Generic;
using System.IO;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Sessions.ResourceTypeResolvers;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceTypeResolverManager
    {
        private readonly Dictionary<string, ResourceType> m_resolversDictionary =
            new Dictionary<string, ResourceType>();

        public ResourceTypeResolverManager(IKernel container)
        {
            ProcessResolvers(container.ResolveAll<ResourceTypeResolverBase>());
        }

        private void ProcessResolvers(ResourceTypeResolverBase[] resolvers)
        {
            foreach (var resolver in resolvers)
            {
                foreach (var fileExtension in resolver.FileExtensions)
                {
                    m_resolversDictionary.Add(fileExtension, resolver.ResolveResourceType);
                }
            }
        }

        public ResourceType Resolve(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension == null)
            {
                throw new ArgumentException(string.Format("File with filename '{0}' does not have extension", fileName));
            }
            var fileExtension = extension.ToLowerInvariant();
            return m_resolversDictionary[fileExtension];
        }
    }
}