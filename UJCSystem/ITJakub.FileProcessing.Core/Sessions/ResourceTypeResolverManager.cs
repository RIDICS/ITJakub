using System;
using System.Collections.Generic;
using System.IO;
using Castle.MicroKernel;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceTypeResolverManager
    {
        private readonly Dictionary<string, ResourceTypeEnum> m_resolversDictionary =
            new Dictionary<string, ResourceTypeEnum>();

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

        public ResourceTypeEnum Resolve(string fileName)
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

    public abstract class ResourceTypeResolverBase
    {
        protected ResourceTypeResolverBase(string[] fileExtensions)
        {
            FileExtensions = fileExtensions;
        }

        public abstract ResourceTypeEnum ResolveResourceType { get; }
        public string[] FileExtensions { get; private set; }
    }

    public class SourceDocumentTypeResolver : ResourceTypeResolverBase
    {
        public SourceDocumentTypeResolver(string[] fileExtensions) : base(fileExtensions)
        {
        }

        public override ResourceTypeEnum ResolveResourceType
        {
            get { return ResourceTypeEnum.SourceDocument; }
        }
    }

    public class ImageTypeResolver : ResourceTypeResolverBase
    {
        public ImageTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceTypeEnum ResolveResourceType
        {
            get { return ResourceTypeEnum.Image; }
        }
    }

    public class TransformationTypeResolver : ResourceTypeResolverBase
    {
        public TransformationTypeResolver(string[] fileExtensions)
            : base(fileExtensions)
        {
        }

        public override ResourceTypeEnum ResolveResourceType
        {
            get { return ResourceTypeEnum.Transformation; }
        }
    }
}